using kiwiDeal.Notifications.Application.DTOs;
using kiwiDeal.Notifications.Application.Events;
using Microsoft.AspNetCore.SignalR;

namespace kiwiDeal.Notifications.Api;

public class NotificationHubContext(IHubContext<NotificationHub> hubContext) : INotificationHubContext
{
    public async Task SendNotificationReceived(
        Guid recipientId,
        NotificationDto notification,
        CancellationToken cancellationToken = default)
    {
        await hubContext.Clients
            .User(recipientId.ToString())
            .SendAsync("NotificationReceived", notification, cancellationToken);
    }
}
