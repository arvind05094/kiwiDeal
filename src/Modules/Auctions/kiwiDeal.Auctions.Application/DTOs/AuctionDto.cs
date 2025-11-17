namespace kiwiDeal.Auctions.Application.DTOs;

public sealed record AuctionDto(
    Guid Id,
    Guid ListingId,
    Guid SellerId,
    decimal StartingPrice,
    decimal? CurrentHighestBid,
    Guid? CurrentHighestBidderId,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    string Status,
    IReadOnlyList<AuctionBidDto> Bids);
