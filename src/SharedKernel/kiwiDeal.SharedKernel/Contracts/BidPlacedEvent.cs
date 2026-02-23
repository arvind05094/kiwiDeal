using kiwiDeal.SharedKernel.Events;

namespace kiwiDeal.SharedKernel.Contracts;

public sealed record BidPlacedEvent(
    Guid AuctionId,
    Guid BidId,
    Guid BidderId,
    decimal Amount,
    DateTimeOffset NewEndTime) : IDomainEvent
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
