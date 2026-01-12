using kiwiDeal.Listings.Domain.Errors;
using kiwiDeal.Listings.Domain.Events;
using kiwiDeal.Listings.Domain.ValueObjects;
using kiwiDeal.SharedKernel.Entities;
using kiwiDeal.SharedKernel.Results;
namespace kiwiDeal.Listings.Domain.Entities;
public sealed class Listing : AggregateRoot
{
    public ListingId Id { get; private set; } = default!;
    public SellerId SellerId { get; private set; } = default!;
    public string Title { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public decimal StartingPrice { get; private set; }
    public ListingStatus Status { get; private set; }
    private readonly List<ListingImage> _images = [];
    public IReadOnlyList<ListingImage> Images => _images.AsReadOnly();
    private Listing() { }
    public static Result<Listing> Create(
        SellerId sellerId,
        string title,
        string description,
        decimal startingPrice)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure<Listing>(Error.ValidationFailed("Title is required."));
        if (string.IsNullOrWhiteSpace(description))
            return Result.Failure<Listing>(Error.ValidationFailed("Description is required."));
        if (startingPrice < 0)
            return Result.Failure<Listing>(Error.ValidationFailed("Starting price must be zero or greater."));
        var now = DateTimeOffset.UtcNow;
        var listing = new Listing
        {
            Id = ListingId.New(),
            SellerId = sellerId,
            Title = title,
            Description = description,
            StartingPrice = startingPrice,
            Status = ListingStatus.Active,
            CreatedAt = now,
            UpdatedAt = now
        };
        listing.RaiseDomainEvent(new ListingCreatedEvent(
            listing.Id.Value,
            listing.SellerId.Value,
            listing.Title,
            listing.StartingPrice));
        return Result.Success(listing);
    }
    public Result Update(string title, string description, decimal startingPrice)
    {
        if (Status == ListingStatus.Sold || Status == ListingStatus.Cancelled)
            return Result.Failure(ListingErrors.AlreadyClosed());
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure(Error.ValidationFailed("Title is required."));
        if (string.IsNullOrWhiteSpace(description))
            return Result.Failure(Error.ValidationFailed("Description is required."));
        if (startingPrice < 0)
            return Result.Failure(Error.ValidationFailed("Starting price must be zero or greater."));
        Title = title;
        Description = description;
        StartingPrice = startingPrice;
        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }
    public Result Close()
    {
        if (Status == ListingStatus.Sold || Status == ListingStatus.Cancelled)
            return Result.Failure(ListingErrors.AlreadyClosed());
        Status = ListingStatus.Cancelled;
        UpdatedAt = DateTimeOffset.UtcNow;
        RaiseDomainEvent(new ListingClosedEvent(
            Id.Value,
            SellerId.Value));
        return Result.Success();
    }
    public Result MarkSold()
    {
        if (Status == ListingStatus.Sold || Status == ListingStatus.Cancelled)
            return Result.Failure(ListingErrors.AlreadyClosed());
        Status = ListingStatus.Sold;
        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }
    public void AddImage(ListingImage image)
    {
        _images.Add(image);
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
