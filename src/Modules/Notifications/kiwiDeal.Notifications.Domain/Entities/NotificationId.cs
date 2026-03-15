using kiwiDeal.SharedKernel.Interfaces;

namespace kiwiDeal.Notifications.Domain.Entities;

public record NotificationId : IStronglyTypedId
{
    public Guid Value { get; }
    private NotificationId(Guid value) { Value = value; }
    public static NotificationId New() => new(Guid.CreateVersion7());
    public static NotificationId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
