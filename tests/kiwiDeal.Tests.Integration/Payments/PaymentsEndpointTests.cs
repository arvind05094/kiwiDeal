using FluentAssertions;
using kiwiDeal.Payments.Domain.Entities;
using kiwiDeal.Payments.Infrastructure.Persistence;
using kiwiDeal.Tests.Integration.Listings;
using kiwiDeal.Tests.Integration.Users;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;

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

        // Go through checkout to set the stripe session id
        var checkoutRequest = new { AuctionId = payment.AuctionId };
        await _client.PostAsJsonAsync("/api/v1/payments/checkout", checkoutRequest);

        // Reload payment to get the stripe session id
        await db.Entry(payment).ReloadAsync();
        var sessionId = payment.StripeSessionId!;

        // Build a signed Stripe webhook payload
        var payload = $$"""
        {
            "id": "evt_test",
            "type": "checkout.session.completed",
            "data": {
                "object": {
                    "id": "{{sessionId}}",
                    "object": "checkout.session"
                }
            }
        }
        """;
        var stripeSignature = CreateMockStripeSignature(payload, "whsec_test_secret");
        var content = new StringContent(payload, System.Text.Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/payments/webhook")
        {
            Content = content
        };
        request.Headers.Add("Stripe-Signature", stripeSignature);

        var response = await _client.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task HandleWebhook_UnknownSessionId_Returns404()
    {
        var sessionId = "cs_test_unknown_session";
        var payload = $$"""
        {
            "id": "evt_test",
            "type": "checkout.session.completed",
            "data": {
                "object": {
                    "id": "{{sessionId}}",
                    "object": "checkout.session"
                }
            }
        }
        """;
        var stripeSignature = CreateMockStripeSignature(payload, "whsec_test_secret");
        var content = new StringContent(payload, System.Text.Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/payments/webhook")
        {
            Content = content
        };
        request.Headers.Add("Stripe-Signature", stripeSignature);

        var response = await _client.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPayment_ExistingPayment_Returns200()
    {
        var auctionId = await SeedPaymentAsync(sellerId: TestAuthHandler.SellerId);

        var response = await _client.GetAsync($"/api/v1/payments/{auctionId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPayment_NonExistentPayment_Returns404()
    {
        var response = await _client.GetAsync($"/api/v1/payments/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private static string CreateMockStripeSignature(string jsonPayload, string secret)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        var signedPayload = $"{timestamp}.{jsonPayload}";
        var rawSecret = secret.StartsWith("whsec_") ? secret["whsec_".Length..] : secret;

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(rawSecret));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(signedPayload));

        var sb = new StringBuilder();
        foreach (var b in hashBytes)
            sb.Append(b.ToString("x2"));

        return $"t={timestamp},v1={sb}";
    }

}


