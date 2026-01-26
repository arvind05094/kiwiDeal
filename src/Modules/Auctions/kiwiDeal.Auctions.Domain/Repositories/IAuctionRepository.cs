using kiwiDeal.Auctions.Domain.Entities;
using kiwiDeal.SharedKernel.Pagination;

namespace kiwiDeal.Auctions.Domain.Repositories;

public interface IAuctionRepository
{
    Task<Auction?> GetByIdAsync(AuctionId id, CancellationToken cancellationToken = default);
    Task<Auction?> GetByListingIdAsync(Guid listingId, CancellationToken cancellationToken = default);
    Task<List<Auction>> GetScheduledReadyToActivateAsync(CancellationToken cancellationToken = default);
    Task<List<Auction>> GetExpiredActiveAuctionsAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<Auction>> GetPagedAsync(int pageNumber, int pageSize, bool endingSoon = false, CancellationToken cancellationToken = default);
    void Add(Auction auction);
}