using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Notifications.Application.Commands;

public record MarkAllNotificationsAsReadCommand(Guid UserId) : IRequest<Result>;
