using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Notifications.Application.Commands;

public record MarkNotificationAsReadCommand(
    Guid NotificationId,
    Guid UserId) : IRequest<Result>;
