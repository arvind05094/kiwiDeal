namespace kiwiDeal.Auctions.Application.DTOs;

public sealed record AuctionWatchlistItemDto(
    Guid AuctionId,
    DateTimeOffset WatchedSince);
