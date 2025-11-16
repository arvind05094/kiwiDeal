namespace kiwiDeal.Auctions.Domain.Entities;

public record AuctionId
{
    public Guid Value { get; }
    private AuctionId(Guid value) { Value = value; }
    public static AuctionId New() => new(Guid.CreateVersion7());
    public static AuctionId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
