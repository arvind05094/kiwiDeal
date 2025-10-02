using MediatR;

namespace kiwiDeal.SharedKernel.Events;

public interface IDomainEvent : INotification
{
    Guid Id { get; }
    DateTimeOffset OccurredOn { get; }
}
