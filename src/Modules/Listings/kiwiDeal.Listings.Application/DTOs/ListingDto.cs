namespace kiwiDeal.Listings.Application.DTOs;

public sealed record ListingDto(
    Guid Id,
    Guid SellerId,
    string Title,
    string Description,
    decimal StartingPrice,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    IReadOnlyList<ListingImageDto> Images);

public sealed record ListingImageDto(
    string Url,
    int DisplayOrder);
