using kiwiDeal.SharedKernel.Events;

namespace kiwiDeal.SharedKernel.Outbox;

public sealed class OutboxMessage : IOutboxMessage
{
    public Guid Id { get; private set; }
    public string EventType { get; private set; } = string.Empty;
    public string Payload { get; private set; } = string.Empty;
    public DateTimeOffset OccurredOn { get; private set; }
    public DateTimeOffset? ProcessedOn { get; private set; }

    private OutboxMessage() { }

    public static OutboxMessage Create(IDomainEvent domainEvent, string payload)
    {
        return new OutboxMessage
        {
            Id = domainEvent.Id,
            EventType = domainEvent.GetType().FullName!,
            Payload = payload,
            OccurredOn = domainEvent.OccurredOn
        };
    }

    public void MarkProcessed(DateTimeOffset processedOn)
    {
        ProcessedOn = processedOn;
    }
}
