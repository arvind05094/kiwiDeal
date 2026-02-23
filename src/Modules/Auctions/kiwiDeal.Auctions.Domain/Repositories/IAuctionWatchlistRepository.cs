using kiwiDeal.Auctions.Domain.Entities;
using kiwiDeal.SharedKernel.Pagination;

namespace kiwiDeal.Auctions.Domain.Repositories;

public interface IAuctionWatchlistRepository
{
    Task<AuctionWatchlist?> GetEntryAsync(Guid userId, Guid auctionId, CancellationToken cancellationToken = default);
    Task<PagedResult<AuctionWatchlist>> GetByUserIdAsync(Guid userId, PaginationParams pagination, CancellationToken cancellationToken = default);
    Task AddAsync(AuctionWatchlist entry, CancellationToken cancellationToken = default);
    void Remove(AuctionWatchlist entry);
}
