using kiwiDeal.SharedKernel.Events;

namespace kiwiDeal.Payments.Domain.Events;

public sealed record PaymentCompletedEvent(
    Guid PaymentId,
    Guid AuctionId,
    Guid WinnerId,
    Guid SellerId,
    decimal Amount) : IDomainEvent
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
