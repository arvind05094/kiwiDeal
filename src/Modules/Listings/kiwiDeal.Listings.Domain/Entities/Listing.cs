using kiwiDeal.Listings.Domain.Enums;
using kiwiDeal.Listings.Domain.Errors;
using kiwiDeal.SharedKernel.Contracts;
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
    public ListingType ListingType { get; private set; }
    public decimal? BuyNowPrice { get; private set; }
    public ListingCategory Category { get; private set; }
    public ListingRegion Region { get; private set; }
    public decimal? SoldAmount { get; private set; }
    public Guid? AuctionId { get; private set; }
    public ListingStatus Status { get; private set; }

    private readonly List<ListingImage> _images = [];
    public IReadOnlyList<ListingImage> Images => _images.AsReadOnly();

    public string SellerName { get; private set; } = default!;
    private Listing() { }

    public void AssignAuction(Guid auctionId)
    {
        AuctionId = auctionId;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public static Result<Listing> Create(
        SellerId sellerId,
        string sellerName,
        string title,
        string description,
        ListingType listingType,
        decimal? buyNowPrice,
        ListingCategory category,
        ListingRegion region)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure<Listing>(Error.ValidationFailed("Title is required."));

        if (string.IsNullOrWhiteSpace(description))
            return Result.Failure<Listing>(Error.ValidationFailed("Description is required."));

        if (listingType == ListingType.FixedPrice && (buyNowPrice is null || buyNowPrice <= 0))
            return Result.Failure<Listing>(Error.ValidationFailed("Buy now price is required for fixed price listings."));

        if (listingType == ListingType.Auction && buyNowPrice is not null)
            return Result.Failure<Listing>(Error.ValidationFailed("Auction listings do not have a buy now price."));

        var now = DateTimeOffset.UtcNow;
        var listing = new Listing
        {
            Id = ListingId.New(),
            SellerId = sellerId,
            SellerName = sellerName,
            Title = title,
            Description = description,
            ListingType = listingType,
            BuyNowPrice = buyNowPrice,
            Category = category,
            Region = region,
            Status = ListingStatus.Active,
            CreatedAt = now,
            UpdatedAt = now
        };

        listing.RaiseDomainEvent(new ListingCreatedEvent(
            listing.Id.Value,
            listing.SellerId.Value,
            listing.Title));

        return Result.Success(listing);
    }

    public Result Update(string title, string description)
    {
        if (Status == ListingStatus.Sold || Status == ListingStatus.Cancelled || Status == ListingStatus.PendingPayment)
            return Result.Failure(ListingErrors.AlreadyClosed());

        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure(Error.ValidationFailed("Title is required."));

        if (string.IsNullOrWhiteSpace(description))
            return Result.Failure(Error.ValidationFailed("Description is required."));

        Title = title;
        Description = description;
        UpdatedAt = DateTimeOffset.UtcNow;

        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status == ListingStatus.Sold || Status == ListingStatus.Cancelled || Status == ListingStatus.PendingPayment)
            return Result.Failure(ListingErrors.AlreadyClosed());
        
        Status = ListingStatus.Cancelled;
        UpdatedAt = DateTimeOffset.UtcNow;

        RaiseDomainEvent(new ListingCancelledEvent(Id.Value, SellerId.Value));

        return Result.Success();
    }

    public Result MarkSold(decimal amount)
    {
        if (Status == ListingStatus.Sold || Status == ListingStatus.Cancelled)
            return Result.Failure(ListingErrors.AlreadyClosed());

        Status = ListingStatus.Sold;
        SoldAmount = amount;
        UpdatedAt = DateTimeOffset.UtcNow;

        RaiseDomainEvent(new ListingSoldEvent(Id.Value, SellerId.Value));

        return Result.Success();
    }

    public Result MarkPendingPayment()
    {
        if (Status != ListingStatus.Active)
            return Result.Failure(ListingErrors.AlreadyClosed());

        Status = ListingStatus.PendingPayment;
        UpdatedAt = DateTimeOffset.UtcNow;

        return Result.Success();
    }

    public Result AddImage(ListingImage image)
    {
        if (_images.Count >= 3)
            return Result.Failure(ListingErrors.TooManyImages());

        _images.Add(image);
        UpdatedAt = DateTimeOffset.UtcNow;

        return Result.Success();
    }
}
