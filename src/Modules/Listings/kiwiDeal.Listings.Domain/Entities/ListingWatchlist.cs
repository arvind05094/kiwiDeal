namespace kiwiDeal.Listings.Domain.Entities;

public sealed class ListingWatchlist
{
    public Guid UserId { get; private set; }
    public ListingId ListingId { get; private set; } = default!;
    public DateTimeOffset CreatedAt { get; private set; }

    public Listing Listing { get; private set; } = default!;
    private ListingWatchlist() { }

    public static ListingWatchlist Create(Guid userId, ListingId listingId)
    {
        return new ListingWatchlist
        {
            UserId = userId,
            ListingId = listingId,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}
