using kiwiDeal.SharedKernel.Events;

namespace kiwiDeal.Auctions.Domain.Events;

public sealed record AuctionCreatedEvent(
    Guid AuctionId,
    Guid ListingId) : IDomainEvent
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
