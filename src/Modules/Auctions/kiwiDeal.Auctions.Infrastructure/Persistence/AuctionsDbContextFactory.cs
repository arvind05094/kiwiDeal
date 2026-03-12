using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace kiwiDeal.Auctions.Infrastructure.Persistence;

public sealed class AuctionsDbContextFactory : IDesignTimeDbContextFactory<AuctionsDbContext>
{
    public AuctionsDbContext CreateDbContext(string[] args)
    {
        var connStr = Environment.GetEnvironmentVariable("ConnectionStrings__AuctionsConnection")
            ?? throw new InvalidOperationException("ConnectionStrings__AuctionsConnection environment variable is not set.");

        var optionsBuilder = new DbContextOptionsBuilder<AuctionsDbContext>();
        optionsBuilder
            .UseNpgsql(connStr)
            .UseSnakeCaseNamingConvention();

        return new AuctionsDbContext(optionsBuilder.Options);
    }
}