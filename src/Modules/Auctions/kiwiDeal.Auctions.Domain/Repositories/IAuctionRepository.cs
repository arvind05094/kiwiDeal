namespace kiwiDeal.Auctions.Domain.Repositories;

using kiwiDeal.Auctions.Domain.Entities;

public interface IAuctionRepository
{
    Task<Auction?> GetByIdAsync(AuctionId id, CancellationToken cancellationToken = default);
    Task<Auction?> GetByListingIdAsync(Guid listingId, CancellationToken cancellationToken = default);
    void Add(Auction auction);
}
