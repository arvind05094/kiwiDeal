namespace kiwiDeal.Listings.Domain.Entities;

public record ListingId
{
    public Guid Value { get; }
    private ListingId(Guid value) { Value = value; }
    public static ListingId New() => new(Guid.CreateVersion7());
    public static ListingId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
