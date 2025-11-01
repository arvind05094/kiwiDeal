namespace kiwiDeal.Listings.Domain.Entities;

public record SellerId
{
    public Guid Value { get; }
    private SellerId(Guid value) { Value = value; }
    public static SellerId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
