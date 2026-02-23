using kiwiDeal.SharedKernel.Events;

namespace kiwiDeal.SharedKernel.Contracts;

public sealed record PaymentCompletedEvent(
    Guid PaymentId,
    Guid? AuctionId,
    Guid ListingId,
    Guid BuyerId,
    Guid SellerId,
    decimal Amount,
    string PaymentType) : IDomainEvent
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
