using kiwiDeal.Listings.Domain.Enums;

namespace kiwiDeal.Listings.Api.Requests;

public sealed record CreateListingRequest(
    string Title,
    string Description,
    ListingType ListingType,
    decimal? BuyNowPrice,
    ListingCategory Category,
    ListingRegion Region);