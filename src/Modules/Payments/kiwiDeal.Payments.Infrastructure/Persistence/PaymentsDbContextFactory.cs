using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace kiwiDeal.Payments.Infrastructure.Persistence;

public sealed class PaymentsDbContextFactory : IDesignTimeDbContextFactory<PaymentsDbContext>
{
    public PaymentsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PaymentsDbContext>();
        optionsBuilder
            .UseNpgsql("Host=localhost;Port=5432;Database=kiwidealddb;Username=kiwiadmin;Password=kiwipassword")
            .UseSnakeCaseNamingConvention();
        return new PaymentsDbContext(optionsBuilder.Options);
    }
}
