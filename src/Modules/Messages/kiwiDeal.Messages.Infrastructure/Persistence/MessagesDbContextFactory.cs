using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace kiwiDeal.Messages.Infrastructure.Persistence;

public sealed class MessagesDbContextFactory : IDesignTimeDbContextFactory<MessagesDbContext>
{
    public MessagesDbContext CreateDbContext(string[] args)
    {
        var connStr = Environment.GetEnvironmentVariable("ConnectionStrings__MessagesConnection")
            ?? throw new InvalidOperationException("ConnectionStrings__MessagesConnection environment variable is not set.");

        var optionsBuilder = new DbContextOptionsBuilder<MessagesDbContext>();
        optionsBuilder
            .UseNpgsql(connStr)
            .UseSnakeCaseNamingConvention();

        return new MessagesDbContext(optionsBuilder.Options);
    }
}
