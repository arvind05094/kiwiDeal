using kiwiDeal.SharedKernel.Entities;

namespace kiwiDeal.Auctions.Domain.Entities;

public sealed class AuctionBid : BaseEntity
{
    public AuctionBidId Id { get; private set; } = default!;
    public AuctionId AuctionId { get; private set; } = default!;
    public Guid BidderId { get; private set; }
    public string BidderName { get; private set; } = default!;
    public decimal Amount { get; private set; }

    private AuctionBid() { }

    internal static AuctionBid Create(AuctionId auctionId, Guid bidderId, string bidderName, decimal amount)
    {
        var now = DateTimeOffset.UtcNow;
        return new AuctionBid
        {
            Id = AuctionBidId.New(),
            AuctionId = auctionId,
            BidderId = bidderId,
            BidderName = bidderName,
            Amount = amount,
            CreatedAt = now,
            UpdatedAt = now
        };
    }
}
