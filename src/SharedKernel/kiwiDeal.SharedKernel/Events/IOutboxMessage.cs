namespace kiwiDeal.SharedKernel.Events;

public interface IOutboxMessage
{
    Guid Id { get; }
    string EventType { get; }
    string Payload { get; }
    DateTimeOffset OccurredOn { get; }
    DateTimeOffset? ProcessedOn { get; }
}
