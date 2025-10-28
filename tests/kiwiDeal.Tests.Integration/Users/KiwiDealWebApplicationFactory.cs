using kiwiDeal.Users.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace kiwiDeal.Tests.Integration.Users;

public class KiwiDealWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder("postgres:17")
        .WithImage("postgres:17")
        .WithDatabase("kiwidealddb_test")
        .WithUsername("kiwiadmin")
        .WithPassword("kiwipassword")
        .Build();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<UsersDbContext>));

            if (descriptor is not null)
                services.Remove(descriptor);

            services.AddDbContext<UsersDbContext>(options =>
                options
                    .UseNpgsql(_dbContainer.GetConnectionString())
                    .UseSnakeCaseNamingConvention());

            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
            db.Database.EnsureCreated();
        });

        builder.UseSetting("JwtSettings:Secret", "this-is-a-super-secret-key-for-kiwi-deal-change-in-production");
        builder.UseSetting("JwtSettings:Issuer", "kiwiDeal");
        builder.UseSetting("JwtSettings:Audience", "kiwiDeal");
        builder.UseSetting("JwtSettings:ExpiryMinutes", "15");
    }
}
