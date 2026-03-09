namespace kiwiDeal.Payments.Application.DTOs;

public sealed record PaymentDto(
    Guid Id,
    Guid? AuctionId,
    Guid ListingId,
    Guid BuyerId,
    Guid SellerId,
    decimal Amount,
    string PaymentType,
    string Status,
    string? StripeSessionId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? PaidAt,
    string? ListingTitle,
    string? ThumbnailUrl);