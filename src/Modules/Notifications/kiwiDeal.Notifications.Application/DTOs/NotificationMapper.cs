using kiwiDeal.Notifications.Domain.Entities;

namespace kiwiDeal.Notifications.Application.DTOs;

public static class NotificationMapper
{
    public static NotificationDto ToDto(Notification notification) => new()
    {
        Id = notification.Id.Value,
        Type = notification.Type,
        Title = notification.Title,
        Message = notification.Message,
        LinkUrl = notification.LinkUrl,
        IsRead = notification.IsRead,
        CreatedAt = notification.CreatedAt
    };
}
