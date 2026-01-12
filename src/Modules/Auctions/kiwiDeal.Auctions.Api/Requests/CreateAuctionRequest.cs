namespace kiwiDeal.Auctions.Api.Requests;

public sealed record CreateAuctionRequest(
    Guid ListingId,
    string ListingTitle,
    decimal StartingPrice,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
