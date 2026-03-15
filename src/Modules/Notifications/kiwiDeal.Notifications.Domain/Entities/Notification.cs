using kiwiDeal.SharedKernel.Entities;

namespace kiwiDeal.Notifications.Domain.Entities;

public sealed class Notification : BaseEntity
{
    private Notification() { }

    public NotificationId Id { get; private set; } = default!;
    public Guid RecipientId { get; private set; }
    public NotificationType Type { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public string? LinkUrl { get; private set; }
    public bool IsRead { get; private set; }

    public static Notification Create(
        Guid recipientId,
        NotificationType type,
        string title,
        string message,
        string? linkUrl)
    {
        var now = DateTimeOffset.UtcNow;
        return new Notification
        {
            Id = NotificationId.New(),
            RecipientId = recipientId,
            Type = type,
            Title = title,
            Message = message,
            LinkUrl = linkUrl,
            IsRead = false,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public void MarkAsRead()
    {
        if (IsRead)
            return;

        IsRead = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
