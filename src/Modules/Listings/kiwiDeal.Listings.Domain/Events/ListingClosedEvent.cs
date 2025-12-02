using kiwiDeal.SharedKernel.Events;

namespace kiwiDeal.Listings.Domain.Events;

public sealed record ListingClosedEvent(
    Guid ListingId,
    Guid SellerId) : IDomainEvent
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
