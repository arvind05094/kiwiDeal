namespace kiwiDeal.Auctions.Api.Requests;

public sealed record CreateAuctionRequest(
    Guid ListingId,
    decimal StartingPrice,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
