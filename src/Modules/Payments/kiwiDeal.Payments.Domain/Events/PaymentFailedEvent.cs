using kiwiDeal.SharedKernel.Events;

namespace kiwiDeal.Payments.Domain.Events;

public sealed record PaymentFailedEvent(
    Guid PaymentId,
    Guid AuctionId,
    Guid WinnerId) : IDomainEvent
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
