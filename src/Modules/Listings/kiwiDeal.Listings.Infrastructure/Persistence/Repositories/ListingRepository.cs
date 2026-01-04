using kiwiDeal.Listings.Domain.Entities;
using kiwiDeal.Listings.Domain.Repositories;
using kiwiDeal.SharedKernel.Pagination;
using Microsoft.EntityFrameworkCore;

namespace kiwiDeal.Listings.Infrastructure.Persistence.Repositories;

public sealed class ListingRepository(ListingsDbContext context) : IListingRepository
{
    public async Task<Listing?> GetByIdAsync(ListingId id, CancellationToken cancellationToken = default)
    {
        return await context.Listings
            .Include(l => l.Images)
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<PagedResult<Listing>> GetAllAsync(PaginationParams pagination, string? searchTerm, CancellationToken cancellationToken = default)
    {
        var query = context.Listings
            .Include(l => l.Images)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(l =>
                EF.Functions.ToTsVector("english", l.Title + " " + l.Description)
                    .Matches(EF.Functions.ToTsQuery("english", term)));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(l => l.CreatedAt)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<Listing>.Create(items, totalCount, pagination);
    }

    public async Task<PagedResult<Listing>> GetBySellerIdAsync(Guid sellerId, PaginationParams pagination, CancellationToken cancellationToken = default)
    {
        var query = context.Listings
            .Include(l => l.Images)
            .Where(l => l.SellerId.Value == sellerId);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(l => l.CreatedAt)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<Listing>.Create(items, totalCount, pagination);
    }

    public async Task AddAsync(Listing listing, CancellationToken cancellationToken = default)
    {
        await context.Listings.AddAsync(listing, cancellationToken);
    }

    public void Update(Listing listing)
    {
        context.Listings.Update(listing);
    }

    public void Delete(Listing listing)
    {
        context.Listings.Remove(listing);
    }
}
