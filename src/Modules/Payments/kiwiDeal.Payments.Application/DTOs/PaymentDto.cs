namespace kiwiDeal.Payments.Application.DTOs;

public sealed record PaymentDto(
    Guid Id,
    Guid AuctionId,
    Guid WinnerId,
    Guid SellerId,
    decimal Amount,
    string Status,
    string? StripeSessionId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? PaidAt);
