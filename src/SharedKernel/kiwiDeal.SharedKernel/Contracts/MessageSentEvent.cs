using kiwiDeal.SharedKernel.Events;

namespace kiwiDeal.SharedKernel.Contracts;

public sealed record MessageSentEvent(
    Guid ConversationId,
    Guid RecipientId,
    Guid SenderId,
    string SenderName,
    string Content) : IDomainEvent
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
