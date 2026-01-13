using kiwiDeal.SharedKernel.Events;

namespace kiwiDeal.Listings.Domain.Events;

public sealed record ListingCreatedEvent(
    Guid ListingId,
    Guid SellerId,
    string Title) : IDomainEvent
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
