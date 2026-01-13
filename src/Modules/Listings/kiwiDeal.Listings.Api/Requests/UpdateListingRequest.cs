namespace kiwiDeal.Listings.Api.Requests;

public sealed record UpdateListingRequest(
    string Title,
    string Description);