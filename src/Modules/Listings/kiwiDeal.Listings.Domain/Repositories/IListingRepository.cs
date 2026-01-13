using kiwiDeal.Listings.Domain.Entities;
using kiwiDeal.SharedKernel.Pagination;

namespace kiwiDeal.Listings.Domain.Repositories;

public interface IListingRepository
{
    Task<Listing?> GetByIdAsync(ListingId id, CancellationToken cancellationToken = default);
    Task<ListingWatchlist?> GetWatchlistEntryAsync(Guid userId, ListingId listingId, CancellationToken cancellationToken = default);
    Task<PagedResult<ListingWatchlist>> GetWatchlistByUserIdAsync(Guid userId, PaginationParams pagination, CancellationToken cancellationToken = default);
    Task AddWatchlistEntryAsync(ListingWatchlist entry, CancellationToken cancellationToken = default);
    void RemoveWatchlistEntry(ListingWatchlist entry);
    Task<PagedResult<Listing>> GetBySellerIdAsync(Guid sellerId, PaginationParams pagination, ListingStatus[]? statuses, CancellationToken cancellationToken = default);
    Task AddAsync(Listing listing, CancellationToken cancellationToken = default);
    void Update(Listing listing);
    Task<PagedResult<Listing>> GetAllAsync(PaginationParams pagination, string? searchTerm, string? category, string? region, string? sortBy, CancellationToken cancellationToken = default);
}
