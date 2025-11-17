namespace kiwiDeal.Auctions.Application.DTOs;

public sealed record AuctionBidDto(
    Guid Id,
    Guid BidderId,
    decimal Amount,
    DateTimeOffset PlacedAt);
