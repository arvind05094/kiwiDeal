using kiwiDeal.SharedKernel.Events;
namespace kiwiDeal.Auctions.Domain.Events;
public sealed record AuctionClosedEvent(
    Guid AuctionId,
    Guid SellerId,
    Guid? WinningBidderId,
    decimal? WinningAmount) : IDomainEvent
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
