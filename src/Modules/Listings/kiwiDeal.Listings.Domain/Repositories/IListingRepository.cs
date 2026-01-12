using kiwiDeal.Listings.Domain.Entities;
using kiwiDeal.SharedKernel.Pagination;

namespace kiwiDeal.Listings.Domain.Repositories;

public interface IListingRepository
{
    Task<Listing?> GetByIdAsync(ListingId id, CancellationToken cancellationToken = default);
    Task<PagedResult<Listing>> GetAllAsync(PaginationParams pagination, string? searchTerm, CancellationToken cancellationToken = default);
    Task<PagedResult<Listing>> GetBySellerIdAsync(Guid sellerId, PaginationParams pagination, ListingStatus[]? statuses, CancellationToken cancellationToken = default);
    Task AddAsync(Listing listing, CancellationToken cancellationToken = default);
    void Update(Listing listing);
}
