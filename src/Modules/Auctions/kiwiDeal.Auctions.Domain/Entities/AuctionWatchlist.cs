namespace kiwiDeal.Auctions.Domain.Entities;

public sealed class AuctionWatchlist
{
    public Guid UserId { get; private set; }
    public Guid AuctionId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private AuctionWatchlist() { }

    public static AuctionWatchlist Create(Guid userId, Guid auctionId)
    {
        return new AuctionWatchlist
        {
            UserId = userId,
            AuctionId = auctionId,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}
