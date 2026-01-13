namespace kiwiDeal.Listings.Application.DTOs;

public sealed record ListingDto(
    Guid Id,
    Guid SellerId,
    string Title,
    string Description,
    string ListingType,
    decimal? BuyNowPrice,
    string Category,
    string Region,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    IReadOnlyList<ListingImageDto> Images);
public sealed record ListingImageDto(
    string Url,
    int DisplayOrder);

public sealed record WatchlistItemDto(
    Guid ListingId,
    string Title,
    string Status,
    string ListingType,
    decimal? BuyNowPrice,
    string? ThumbnailUrl,
    DateTimeOffset WatchedSince);
