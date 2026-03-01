namespace kiwiDeal.Listings.Domain.Entities;

using kiwiDeal.SharedKernel.Interfaces;

public record ListingId : IStronglyTypedId
{
    public Guid Value { get; }
    private ListingId(Guid value) { Value = value; }
    public static ListingId New() => new(Guid.CreateVersion7());
    public static ListingId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
