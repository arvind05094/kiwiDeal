using kiwiDeal.Listings.Domain.Repositories;

namespace kiwiDeal.Listings.Infrastructure.Persistence;

public sealed class ListingsUnitOfWork(ListingsDbContext dbContext) : IListingsUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => dbContext.SaveChangesAsync(cancellationToken);
}
