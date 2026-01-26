using kiwiDeal.Auctions.Domain.Entities;
using kiwiDeal.Auctions.Domain.Repositories;
using kiwiDeal.SharedKernel.Pagination;
using Microsoft.EntityFrameworkCore;

namespace kiwiDeal.Auctions.Infrastructure.Persistence.Repositories;

public sealed class AuctionRepository(AuctionsDbContext context) : IAuctionRepository
{
    public async Task<Auction?> GetByIdAsync(AuctionId id, CancellationToken cancellationToken = default)
    {
        return await context.Auctions
            .Include(a => a.Bids)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<Auction?> GetByListingIdAsync(Guid listingId, CancellationToken cancellationToken = default)
    {
        return await context.Auctions
            .Include(a => a.Bids)
            .FirstOrDefaultAsync(a => a.ListingId == listingId, cancellationToken);
    }

    public async Task<PagedResult<Auction>> GetPagedAsync(int pageNumber, int pageSize, bool endingSoon = false, CancellationToken cancellationToken = default)
    {
        var pagination = new PaginationParams(pageNumber, pageSize);
        var query = context.Auctions
            .Where(a => a.Status == AuctionStatus.Active || a.Status == AuctionStatus.Scheduled)
            .AsQueryable();

        if (endingSoon)
        {
            var cutoff = DateTimeOffset.UtcNow.AddHours(24);
            query = query.Where(a => a.Status == AuctionStatus.Active && a.EndTime <= cutoff);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(a => a.EndTime)
            .Skip(pagination.Skip)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<Auction>.Create(items, totalCount, pagination);
    }

    public async Task<List<Auction>> GetScheduledReadyToActivateAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        return await context.Auctions
            .Where(a => a.Status == AuctionStatus.Scheduled && a.StartTime <= now)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Auction>> GetExpiredActiveAuctionsAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        return await context.Auctions
            .Where(a => a.Status == AuctionStatus.Active && a.EndTime <= now)
            .ToListAsync(cancellationToken);
    }

    public void Add(Auction auction)
    {
        context.Auctions.Add(auction);
    }
}