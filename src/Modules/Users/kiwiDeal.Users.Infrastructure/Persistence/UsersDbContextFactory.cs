using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace kiwiDeal.Users.Infrastructure.Persistence;

public sealed class UsersDbContextFactory : IDesignTimeDbContextFactory<UsersDbContext>
{
    public UsersDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../kiwiDeal.Api"))
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddUserSecrets<UsersDbContextFactory>(optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("UsersConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
            connectionString = "Host=localhost;Port=5432;Database=kiwidealddb;Username=kiwiadmin;Password=kiwipassword";

        var optionsBuilder = new DbContextOptionsBuilder<UsersDbContext>();
        optionsBuilder
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention();

        return new UsersDbContext(optionsBuilder.Options);
    }
}
