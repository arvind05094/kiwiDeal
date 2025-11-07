namespace kiwiDeal.Listings.Api.Requests;

public sealed record CreateListingRequest(
    string Title,
    string Description,
    decimal StartingPrice);
