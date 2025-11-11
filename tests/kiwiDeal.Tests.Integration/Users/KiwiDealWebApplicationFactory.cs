using kiwiDeal.Listings.Infrastructure.Persistence;
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
                ["ConnectionStrings:AzureBlobStorage"] = "UseDevelopmentStorage=true"
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
        });

        builder.UseSetting("JwtSettings:Secret", "this-is-a-super-secret-key-for-kiwi-deal-change-in-production");
        builder.UseSetting("JwtSettings:Issuer", "kiwiDeal");
        builder.UseSetting("JwtSettings:Audience", "kiwiDeal");
        builder.UseSetting("JwtSettings:ExpiryMinutes", "15");
        builder.UseSetting("ConnectionStrings:UsersConnection", ConnectionString);
        builder.UseSetting("ConnectionStrings:ListingsConnection", ConnectionString);
    }

    public HttpClient CreateSellerClient()
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Add("X-Integration-Test", "true");
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