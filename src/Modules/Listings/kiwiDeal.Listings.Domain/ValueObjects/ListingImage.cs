namespace kiwiDeal.Listings.Domain.ValueObjects;

public record ListingImage
{
    public string Url { get; }
    public int DisplayOrder { get; }

    private ListingImage(string url, int displayOrder)
    {
        Url = url;
        DisplayOrder = displayOrder;
    }

    public static ListingImage Create(string url, int displayOrder) =>
        new(url, displayOrder);
}
