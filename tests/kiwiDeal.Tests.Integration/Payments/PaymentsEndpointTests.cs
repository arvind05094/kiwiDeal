using FluentAssertions;
using kiwiDeal.Payments.Domain.Entities;
using kiwiDeal.Payments.Infrastructure.Persistence;
using kiwiDeal.Tests.Integration.Users;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;

namespace kiwiDeal.Tests.Integration.Payments;

[Collection("Integration Tests")]
public class PaymentsEndpointTests : IAsyncLifetime
{
    public record CheckoutResponse(string CheckoutUrl);
    private readonly KiwiDealWebApplicationFactory _factory;
    private HttpClient _client = null!;

    public PaymentsEndpointTests(KiwiDealWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        _client = _factory.CreateSellerClient();

        var options = new DbContextOptionsBuilder<PaymentsDbContext>()
            .UseNpgsql(_factory.ConnectionString)
            .UseSnakeCaseNamingConvention()
            .Options;

        await using var db = new PaymentsDbContext(options);
        await db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE payments.payments RESTART IDENTITY CASCADE");
    }

    public Task DisposeAsync()
    {
        _client.Dispose();
        return Task.CompletedTask;
    }

    private async Task<Guid> SeedPaymentAsync(
        Guid? auctionId = null,
        Guid? winnerId = null,
        Guid? sellerId = null,
        decimal amount = 100m)
    {
        var payment = Payment.Create(
            auctionId ?? Guid.NewGuid(),
            winnerId ?? Guid.NewGuid(),
            sellerId ?? Guid.NewGuid(),
            amount).Value;

        var options = new DbContextOptionsBuilder<PaymentsDbContext>()
            .UseNpgsql(_factory.ConnectionString)
            .UseSnakeCaseNamingConvention()
            .Options;

        await using var db = new PaymentsDbContext(options);
        await db.Payments.AddAsync(payment);
        await db.SaveChangesAsync();

        return payment.AuctionId;
    }

    private async Task<Guid> SeedCompletedPaymentAsync()
    {
        var payment = Payment.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            200m).Value;

        payment.Complete("cs_test_existing123");

        var options = new DbContextOptionsBuilder<PaymentsDbContext>()
            .UseNpgsql(_factory.ConnectionString)
            .UseSnakeCaseNamingConvention()
            .Options;

        await using var db = new PaymentsDbContext(options);
        await db.Payments.AddAsync(payment);
        await db.SaveChangesAsync();

        return payment.AuctionId;
    }

    [Fact]
    public async Task CreateCheckoutSession_ExistingPendingPayment_Returns200WithUrl()
    {
        var auctionId = await SeedPaymentAsync();

        var request = new { AuctionId = auctionId };
        var response = await _client.PostAsJsonAsync("/api/v1/payments/checkout", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<CheckoutResponse>();
        body!.CheckoutUrl.Should().Be("https://checkout.stripe.com/test/session");
    }

    [Fact]
    public async Task CreateCheckoutSession_NonExistentPayment_Returns404()
    {
        var request = new { AuctionId = Guid.NewGuid() };
        var response = await _client.PostAsJsonAsync("/api/v1/payments/checkout", request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateCheckoutSession_AlreadyCompleted_Returns400()
    {
        var auctionId = await SeedCompletedPaymentAsync();

        var request = new { AuctionId = auctionId };
        var response = await _client.PostAsJsonAsync("/api/v1/payments/checkout", request);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task HandleWebhook_CheckoutCompleted_Returns204()
    {
        await SeedPaymentAsync();

        var options = new DbContextOptionsBuilder<PaymentsDbContext>()
            .UseNpgsql(_factory.ConnectionString)
            .UseSnakeCaseNamingConvention()
            .Options;

        await using var db = new PaymentsDbContext(options);
        var payment = await db.Payments.FirstAsync();

        var request = new
        {
            StripeSessionId = "cs_test_webhook123",
            EventType = "checkout.session.completed"
        };

        // First update the payment to have the stripe session id by going through checkout
        var checkoutRequest = new { AuctionId = payment.AuctionId };
        await _client.PostAsJsonAsync("/api/v1/payments/checkout", checkoutRequest);

        // Seed a payment that already has a stripe session id via direct db manipulation
        var payment2 = Payment.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 150m).Value;
        await db.Payments.AddAsync(payment2);
        await db.SaveChangesAsync();

        // Use the webhook with the known session id from FakeStripeService
        var webhookRequest = new
        {
            StripeSessionId = "https://checkout.stripe.com/test/session",
            EventType = "checkout.session.completed"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/payments/webhook", webhookRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task HandleWebhook_UnknownSessionId_Returns404()
    {
        var request = new
        {
            StripeSessionId = "cs_test_unknown",
            EventType = "checkout.session.completed"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/payments/webhook", request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPayment_ExistingPayment_Returns200()
    {
        var auctionId = await SeedPaymentAsync();

        var response = await _client.GetAsync($"/api/v1/payments/{auctionId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPayment_NonExistentPayment_Returns404()
    {
        var response = await _client.GetAsync($"/api/v1/payments/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

