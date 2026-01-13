namespace kiwiDeal.Auctions.Application.DTOs;

public sealed record AuctionBidDto(
    Guid Id,
    Guid BidderId,
    string BidderName,
    decimal Amount,
    DateTimeOffset CreatedAt);