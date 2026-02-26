using kiwiDeal.Auctions.Domain.Repositories;

namespace kiwiDeal.Auctions.Infrastructure.Persistence;

public sealed class AuctionsUnitOfWork(AuctionsDbContext dbContext) : IAuctionsUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => dbContext.SaveChangesAsync(cancellationToken);
}
