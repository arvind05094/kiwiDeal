using Asp.Versioning;
using kiwiDeal.Notifications.Application.Commands;
using kiwiDeal.Notifications.Application.Queries;
using kiwiDeal.SharedKernel.Interfaces;
using kiwiDeal.SharedKernel.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace kiwiDeal.Notifications.Api;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/notifications")]
[Authorize]
public sealed class NotificationsController(ISender sender, ICurrentUser currentUser) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetNotifications(CancellationToken ct)
    {
        var query = new GetNotificationsQuery(currentUser.Id!.Value);
        var result = await sender.Send(query, ct);
        if (result.IsFailure) return result.Error.ToProblemDetails();
        return Ok(result.Value);
    }

    [HttpPost("{notificationId:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid notificationId, CancellationToken ct)
    {
        var command = new MarkNotificationAsReadCommand(notificationId, currentUser.Id!.Value);
        var result = await sender.Send(command, ct);
        if (result.IsFailure) return result.Error.ToProblemDetails();
        return NoContent();
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken ct)
    {
        var command = new MarkAllNotificationsAsReadCommand(currentUser.Id!.Value);
        var result = await sender.Send(command, ct);
        if (result.IsFailure) return result.Error.ToProblemDetails();
        return NoContent();
    }
}
