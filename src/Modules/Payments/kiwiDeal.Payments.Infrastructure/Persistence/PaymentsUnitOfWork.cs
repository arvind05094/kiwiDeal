using kiwiDeal.Payments.Domain.Repositories;

namespace kiwiDeal.Payments.Infrastructure.Persistence;

public sealed class PaymentsUnitOfWork(PaymentsDbContext dbContext) : IPaymentsUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => dbContext.SaveChangesAsync(cancellationToken);
}
