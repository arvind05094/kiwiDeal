using kiwiDeal.Auctions.Domain.Errors;
using kiwiDeal.Auctions.Domain.Events;
using kiwiDeal.SharedKernel.Entities;
using kiwiDeal.SharedKernel.Results;

namespace kiwiDeal.Auctions.Domain.Entities;

public sealed class Auction : AggregateRoot
{
    private const int BidExtensionThresholdMinutes = 5;
    private const int BidExtensionMinutes = 5;

    public AuctionId Id { get; private set; } = default!;
    public Guid ListingId { get; private set; }
    public string ListingTitle { get; private set; } = default!;
    public Guid SellerId { get; private set; }
    public decimal StartingPrice { get; private set; }
    public decimal? CurrentHighestBid { get; private set; }
    public Guid? CurrentHighestBidderId { get; private set; }
    public DateTimeOffset StartTime { get; private set; }
    public DateTimeOffset EndTime { get; private set; }
    public AuctionStatus Status { get; private set; }

    private readonly List<AuctionBid> _bids = [];
    public IReadOnlyList<AuctionBid> Bids => _bids.AsReadOnly();

    private Auction() { }

    public static Result<Auction> Create(
        Guid listingId,
        string listingTitle,
        Guid sellerId,
        decimal startingPrice,
        DateTimeOffset startTime,
        DateTimeOffset endTime)
    {
        if (string.IsNullOrWhiteSpace(listingTitle))
            return Result.Failure<Auction>(Error.ValidationFailed("Listing title is required."));
        if (startingPrice < 0)
            return Result.Failure<Auction>(Error.ValidationFailed("Starting price must be zero or greater."));
        if (endTime <= startTime)
            return Result.Failure<Auction>(Error.ValidationFailed("End time must be after start time."));

        var now = DateTimeOffset.UtcNow;
        var auction = new Auction
        {
            Id = AuctionId.New(),
            ListingId = listingId,
            ListingTitle = listingTitle,
            SellerId = sellerId,
            StartingPrice = startingPrice,
            StartTime = startTime,
            EndTime = endTime,
            Status = AuctionStatus.Scheduled,
            CreatedAt = now,
            UpdatedAt = now
        };

        return Result.Success(auction);
    }

    public Result PlaceBid(Guid bidderId, string bidderName, decimal amount)
    {
        var now = DateTimeOffset.UtcNow;

        if (Status == AuctionStatus.Scheduled)
            return Result.Failure(AuctionErrors.NotStarted());

        if (Status == AuctionStatus.Closed)
            return Result.Failure(AuctionErrors.AlreadyClosed());

        if (now > EndTime)
            return Result.Failure(AuctionErrors.AlreadyClosed());

        if (bidderId == SellerId)
            return Result.Failure(AuctionErrors.BidderIsSeller());

        var minimumBid = CurrentHighestBid ?? StartingPrice;
        if (amount <= minimumBid)
            return Result.Failure(AuctionErrors.BidTooLow(minimumBid));

        var bid = AuctionBid.Create(Id, bidderId, bidderName, amount);
        _bids.Add(bid);

        CurrentHighestBid = amount;
        CurrentHighestBidderId = bidderId;

        var timeRemaining = EndTime - now;
        if (timeRemaining.TotalMinutes < BidExtensionThresholdMinutes)
            EndTime = EndTime.AddMinutes(BidExtensionMinutes);

        UpdatedAt = now;

        RaiseDomainEvent(new BidPlacedEvent(
            Id.Value,
            bid.Id.Value,
            bidderId,
            amount,
            EndTime));

        return Result.Success();
    }

    public Result Close()
    {
        if (Status == AuctionStatus.Closed)
            return Result.Failure(AuctionErrors.AlreadyClosed());

        Status = AuctionStatus.Closed;
        UpdatedAt = DateTimeOffset.UtcNow;

        RaiseDomainEvent(new AuctionClosedEvent(
            Id.Value,
            ListingId,
            SellerId,
            CurrentHighestBidderId,
            CurrentHighestBid));

        return Result.Success();
    }

    public Result Activate()
    {
        if (Status == AuctionStatus.Closed)
            return Result.Failure(AuctionErrors.AlreadyClosed());

        Status = AuctionStatus.Active;
        UpdatedAt = DateTimeOffset.UtcNow;

        return Result.Success();
    }
}

