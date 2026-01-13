namespace kiwiDeal.Payments.Api.Requests;

public sealed record CreateBuyNowCheckoutSessionRequest(
    Guid ListingId,
    Guid SellerId,
    decimal Amount);