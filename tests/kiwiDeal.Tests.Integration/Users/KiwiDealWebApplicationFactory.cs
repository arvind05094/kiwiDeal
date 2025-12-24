using kiwiDeal.Auctions.Infrastructure.Persistence;
using kiwiDeal.Listings.Infrastructure.Persistence;
using kiwiDeal.Payments.Application;
using kiwiDeal.Payments.Infrastructure.Persistence;
using kiwiDeal.SharedKernel.Results;
using kiwiDeal.Tests.Integration.Listings;
using kiwiDeal.Users.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace kiwiDeal.Tests.Integration.Users;

public class KiwiDealWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private PostgreSqlContainer? _container;

    public string ConnectionString => _container!.GetConnectionString() + ";Pooling=false";

    public async Task InitializeAsync()
    {
        _container = new PostgreSqlBuilder("postgres:17")
            .WithDatabase("kiwidealddb_test")
            .WithUsername("kiwiadmin")
            .WithPassword("kiwipassword")
            .Build();

        await _container.StartAsync();

        var usersOptions = new DbContextOptionsBuilder<UsersDbContext>()
            .UseNpgsql(ConnectionString)
            .UseSnakeCaseNamingConvention()
            .Options;

        await using var usersDb = new UsersDbContext(usersOptions);
        await usersDb.Database.MigrateAsync();

        var listingsOptions = new DbContextOptionsBuilder<ListingsDbContext>()
            .UseNpgsql(ConnectionString)
            .UseSnakeCaseNamingConvention()
            .Options;

        await using var listingsDb = new ListingsDbContext(listingsOptions);
        await listingsDb.Database.MigrateAsync();

        var auctionsOptions = new DbContextOptionsBuilder<AuctionsDbContext>()
            .UseNpgsql(ConnectionString)
            .UseSnakeCaseNamingConvention()
            .Options;

        await using var auctionsDb = new AuctionsDbContext(auctionsOptions);
        await auctionsDb.Database.MigrateAsync();

        var paymentsOptions = new DbContextOptionsBuilder<PaymentsDbContext>()
            .UseNpgsql(ConnectionString)
            .UseSnakeCaseNamingConvention()
            .Options;

        await using var paymentsDb = new PaymentsDbContext(paymentsOptions);
        await paymentsDb.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        if (_container is not null)
            await _container.StopAsync();
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:UsersConnection"] = ConnectionString,
                ["ConnectionStrings:ListingsConnection"] = ConnectionString,
                ["ConnectionStrings:AuctionsConnection"] = ConnectionString,
                ["ConnectionStrings:AzureBlobStorage"] = "UseDevelopmentStorage=true",
                ["ConnectionStrings:PaymentsConnection"] = ConnectionString,
            });
        });

        builder.ConfigureServices(services =>
        {
            var usersDescriptors = services.Where(
                d => d.ServiceType == typeof(DbContextOptions<UsersDbContext>)
                  || d.ServiceType == typeof(UsersDbContext)).ToList();
            foreach (var d in usersDescriptors)
                services.Remove(d);

            services.AddDbContext<UsersDbContext>(options =>
                options
                    .UseNpgsql(ConnectionString)
                    .UseSnakeCaseNamingConvention());

            var listingsDescriptors = services.Where(
                d => d.ServiceType == typeof(DbContextOptions<ListingsDbContext>)
                  || d.ServiceType == typeof(ListingsDbContext)).ToList();
            foreach (var d in listingsDescriptors)
                services.Remove(d);

            services.AddDbContext<ListingsDbContext>(options =>
                options
                    .UseNpgsql(ConnectionString)
                    .UseSnakeCaseNamingConvention());

            var auctionsDescriptors = services.Where(
                d => d.ServiceType == typeof(DbContextOptions<AuctionsDbContext>)
                  || d.ServiceType == typeof(AuctionsDbContext)).ToList();
            foreach (var d in auctionsDescriptors)
                services.Remove(d);

            services.AddDbContext<AuctionsDbContext>(options =>
                options
                    .UseNpgsql(ConnectionString)
                    .UseSnakeCaseNamingConvention());

            var paymentsDescriptors = services.Where(
                d => d.ServiceType == typeof(DbContextOptions<PaymentsDbContext>)
                  || d.ServiceType == typeof(PaymentsDbContext)).ToList();
            foreach (var d in paymentsDescriptors)
                services.Remove(d);

            services.AddDbContext<PaymentsDbContext>(options =>
                options
                    .UseNpgsql(ConnectionString)
                    .UseSnakeCaseNamingConvention());

            var stripeDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IStripeService));
            if (stripeDescriptor is not null)
                services.Remove(stripeDescriptor);

            var webhookVerifierDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IStripeWebhookVerifier));
            if (webhookVerifierDescriptor is not null)
                services.Remove(webhookVerifierDescriptor);

            services.AddScoped<IStripeWebhookVerifier, FakeStripeWebhookVerifier>();

            services.AddScoped<IStripeService, FakeStripeService>();



            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                "Test", options => { });

            services.AddScoped<kiwiDeal.Users.Domain.Repositories.IUserRepository,
                kiwiDeal.Users.Infrastructure.Persistence.Repositories.UserRepository>();
            services.AddScoped<kiwiDeal.Users.Domain.Repositories.IUsersUnitOfWork, IntegrationTestUsersUnitOfWork>();
            services.AddScoped<kiwiDeal.Listings.Domain.Repositories.IListingsUnitOfWork, IntegrationTestListingsUnitOfWork>();
            services.AddScoped<kiwiDeal.Auctions.Domain.Repositories.IAuctionsUnitOfWork, IntegrationTestAuctionsUnitOfWork>();
            services.AddScoped<kiwiDeal.Payments.Domain.Repositories.IPaymentsUnitOfWork, IntegrationTestPaymentsUnitOfWork>();

        });

        builder.UseSetting("JwtSettings:Secret", "this-is-a-super-secret-key-for-kiwi-deal-change-in-production");
        builder.UseSetting("JwtSettings:Issuer", "kiwiDeal");
        builder.UseSetting("JwtSettings:Audience", "kiwiDeal");
        builder.UseSetting("JwtSettings:ExpiryMinutes", "15");
        builder.UseSetting("ConnectionStrings:UsersConnection", ConnectionString);
        builder.UseSetting("ConnectionStrings:ListingsConnection", ConnectionString);
        builder.UseSetting("ConnectionStrings:AuctionsConnection", ConnectionString);
        builder.UseSetting("ConnectionStrings:PaymentsConnection", ConnectionString);
        builder.UseSetting("Stripe:WebhookSecret", "whsec_test_secret");
    }

    public HttpClient CreateSellerClient()
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Add("X-Integration-Test", "true");
        return client;
    }

    public HttpClient CreateBidderClient()
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Add("X-Integration-Bidder", "true");
        return client;
    }
}

public sealed class IntegrationTestUsersUnitOfWork(UsersDbContext usersDb)
    : kiwiDeal.Users.Domain.Repositories.IUsersUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await usersDb.SaveChangesAsync(cancellationToken);
}

public sealed class IntegrationTestListingsUnitOfWork(ListingsDbContext listingsDb)
    : kiwiDeal.Listings.Domain.Repositories.IListingsUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await listingsDb.SaveChangesAsync(cancellationToken);
}

public sealed class IntegrationTestAuctionsUnitOfWork(AuctionsDbContext auctionsDb)
    : kiwiDeal.Auctions.Domain.Repositories.IAuctionsUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await auctionsDb.SaveChangesAsync(cancellationToken);
}

public sealed class FakeStripeService : IStripeService
{
    public Task<Result<StripeCheckoutSession>> CreateCheckoutSessionAsync(
        Guid paymentId,
        decimal amount,
        CancellationToken cancellationToken = default)
        => Task.FromResult(Result.Success(new StripeCheckoutSession("cs_test_fake_session_id", "https://checkout.stripe.com/test/session")));
}

public sealed class IntegrationTestPaymentsUnitOfWork(PaymentsDbContext paymentsDb)
    : kiwiDeal.Payments.Domain.Repositories.IPaymentsUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await paymentsDb.SaveChangesAsync(cancellationToken);
        return 0;
    }
}

public sealed class FakeStripeWebhookVerifier : IStripeWebhookVerifier
{
    public Result<StripeWebhookEvent> VerifyAndParse(string payload, string stripeSignature)
    {
        try
        {
            var json = System.Text.Json.JsonDocument.Parse(payload);
            var eventType = json.RootElement.GetProperty("type").GetString()!;
            var sessionId = json.RootElement
                .GetProperty("data")
                .GetProperty("object")
                .GetProperty("id")
                .GetString()!;
            return Result.Success(new StripeWebhookEvent(sessionId, eventType));
        }
        catch
        {
            return Result.Failure<StripeWebhookEvent>(Error.ValidationFailed("Invalid webhook payload."));
        }
    }
}