using kiwiDeal.Auctions.Domain.Entities;
using kiwiDeal.Auctions.Domain.Repositories;
using kiwiDeal.SharedKernel.Pagination;
using Microsoft.EntityFrameworkCore;

namespace kiwiDeal.Auctions.Infrastructure.Persistence.Repositories;

public sealed class AuctionWatchlistRepository(AuctionsDbContext context) : IAuctionWatchlistRepository
{
    public async Task<AuctionWatchlist?> GetEntryAsync(Guid userId, Guid auctionId, CancellationToken cancellationToken = default)
    {
        return await context.AuctionWatchlists
            .FirstOrDefaultAsync(w => w.UserId == userId && w.AuctionId == auctionId, cancellationToken);
    }

    public async Task<PagedResult<AuctionWatchlist>> GetByUserIdAsync(Guid userId, PaginationParams pagination, CancellationToken cancellationToken = default)
    {
        var query = context.AuctionWatchlists
            .Where(w => w.UserId == userId);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(w => w.CreatedAt)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<AuctionWatchlist>.Create(items, totalCount, pagination);
    }

    public async Task AddAsync(AuctionWatchlist entry, CancellationToken cancellationToken = default)
    {
        await context.AuctionWatchlists.AddAsync(entry, cancellationToken);
    }

    public void Remove(AuctionWatchlist entry)
    {
        context.AuctionWatchlists.Remove(entry);
    }
}
