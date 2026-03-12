using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace kiwiDeal.Listings.Infrastructure.Persistence;

public sealed class ListingsDbContextFactory : IDesignTimeDbContextFactory<ListingsDbContext>
{
    public ListingsDbContext CreateDbContext(string[] args)
    {
        var connStr = Environment.GetEnvironmentVariable("ConnectionStrings__ListingsConnection")
            ?? throw new InvalidOperationException("ConnectionStrings__ListingsConnection environment variable is not set.");

        var optionsBuilder = new DbContextOptionsBuilder<ListingsDbContext>();
        optionsBuilder
            .UseNpgsql(connStr)
            .UseSnakeCaseNamingConvention();

        return new ListingsDbContext(optionsBuilder.Options);
    }
}
