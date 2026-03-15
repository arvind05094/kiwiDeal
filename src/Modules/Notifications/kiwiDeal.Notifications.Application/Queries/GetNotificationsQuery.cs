using kiwiDeal.Notifications.Application.DTOs;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Notifications.Application.Queries;

public record GetNotificationsQuery(Guid UserId) : IRequest<Result<NotificationsResponseDto>>;
