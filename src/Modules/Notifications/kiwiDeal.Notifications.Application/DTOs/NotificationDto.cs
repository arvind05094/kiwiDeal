using kiwiDeal.Notifications.Domain.Entities;

namespace kiwiDeal.Notifications.Application.DTOs;

public sealed class NotificationDto
{
    public Guid Id { get; init; }
    public NotificationType Type { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public string? LinkUrl { get; init; }
    public bool IsRead { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}

public sealed class NotificationsResponseDto
{
    public List<NotificationDto> Items { get; init; } = [];
    public int UnreadCount { get; init; }
}
