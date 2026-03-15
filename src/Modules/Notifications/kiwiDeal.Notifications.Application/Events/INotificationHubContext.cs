using kiwiDeal.Notifications.Application.DTOs;

namespace kiwiDeal.Notifications.Application.Events;

public interface INotificationHubContext
{
    Task SendNotificationReceived(
        Guid recipientId,
        NotificationDto notification,
        CancellationToken cancellationToken = default);
}
