using kiwiDeal.Listings.Domain.Entities;
using kiwiDeal.Listings.Domain.Enums;
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

    public async Task<PagedResult<Listing>> GetAllAsync(PaginationParams pagination, string? searchTerm, string? category, string? region, string? sortBy, CancellationToken cancellationToken = default)
    {
        var query = context.Listings
            .Include(l => l.Images)
            .Where(l => l.Status == ListingStatus.Active)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(l =>
                EF.Functions.ToTsVector("english", l.Title + " " + l.Description)
                    .Matches(EF.Functions.ToTsQuery("english", term)));
        }

        if (!string.IsNullOrWhiteSpace(category) && Enum.TryParse<ListingCategory>(category, true, out var cat))
            query = query.Where(l => l.Category == cat);

        if (!string.IsNullOrWhiteSpace(region) && Enum.TryParse<ListingRegion>(region, true, out var reg))
            query = query.Where(l => l.Region == reg);

        query = sortBy?.ToLower() switch
        {
            "price_asc" => query.OrderBy(l => l.BuyNowPrice),
            "price_desc" => query.OrderByDescending(l => l.BuyNowPrice),
            _ => query.OrderByDescending(l => l.CreatedAt)
        };

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<Listing>.Create(items, totalCount, pagination);
    }

    public async Task<ListingWatchlist?> GetWatchlistEntryAsync(Guid userId, ListingId listingId, CancellationToken cancellationToken = default)
    {
        return await context.ListingWatchlists
            .FirstOrDefaultAsync(w => w.UserId == userId && w.ListingId == listingId, cancellationToken);
    }

    public async Task<PagedResult<ListingWatchlist>> GetWatchlistByUserIdAsync(Guid userId, PaginationParams pagination, CancellationToken cancellationToken = default)
    {
        var query = context.ListingWatchlists
            .Include(w => w.Listing)
            .ThenInclude(l => l.Images)
            .Where(w => w.UserId == userId);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(w => w.CreatedAt)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<ListingWatchlist>.Create(items, totalCount, pagination);
    }

    public async Task AddWatchlistEntryAsync(ListingWatchlist entry, CancellationToken cancellationToken = default)
    {
        await context.ListingWatchlists.AddAsync(entry, cancellationToken);
    }

    public void RemoveWatchlistEntry(ListingWatchlist entry)
    {
        context.ListingWatchlists.Remove(entry);
    }

    public async Task<PagedResult<Listing>> GetBySellerIdAsync(Guid sellerId, PaginationParams pagination, ListingStatus[]? statuses, CancellationToken cancellationToken = default)
    {
        var query = context.Listings
            .Include(l => l.Images)
            .Where(l => l.SellerId == SellerId.From(sellerId));

        if (statuses is { Length: > 0 })
            query = query.Where(l => statuses.Contains(l.Status));

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

}
