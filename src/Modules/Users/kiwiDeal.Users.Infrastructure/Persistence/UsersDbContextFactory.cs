using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace kiwiDeal.Users.Infrastructure.Persistence;

public sealed class UsersDbContextFactory : IDesignTimeDbContextFactory<UsersDbContext>
{
    public UsersDbContext CreateDbContext(string[] args)
    {
        var connStr = Environment.GetEnvironmentVariable("ConnectionStrings__UsersConnection")
            ?? throw new InvalidOperationException("ConnectionStrings__UsersConnection environment variable is not set.");

        var optionsBuilder = new DbContextOptionsBuilder<UsersDbContext>();
        optionsBuilder
            .UseNpgsql(connStr)
            .UseSnakeCaseNamingConvention();

        return new UsersDbContext(optionsBuilder.Options);
    }
}
