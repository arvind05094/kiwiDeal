using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace kiwiDeal.Notifications.Api;

[Authorize]
public class NotificationHub : Hub
{
}
