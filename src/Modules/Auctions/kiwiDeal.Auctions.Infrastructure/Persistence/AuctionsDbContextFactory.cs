using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace kiwiDeal.Auctions.Infrastructure.Persistence;

public sealed class AuctionsDbContextFactory : IDesignTimeDbContextFactory<AuctionsDbContext>
{
    public AuctionsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuctionsDbContext>();

        optionsBuilder
            .UseNpgsql("Host=localhost;Port=5432;Database=kiwidealddb;Username=kiwiadmin;Password=kiwipassword")
            .UseSnakeCaseNamingConvention();

        return new AuctionsDbContext(optionsBuilder.Options);
    }
}
