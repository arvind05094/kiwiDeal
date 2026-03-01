using kiwiDeal.SharedKernel.Interfaces;

namespace kiwiDeal.Auctions.Domain.Entities;

public record AuctionBidId : IStronglyTypedId
{
    public Guid Value { get; }
    private AuctionBidId(Guid value) { Value = value; }
    public static AuctionBidId New() => new(Guid.CreateVersion7());
    public static AuctionBidId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
