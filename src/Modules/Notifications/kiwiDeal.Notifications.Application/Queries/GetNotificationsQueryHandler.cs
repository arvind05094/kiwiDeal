using kiwiDeal.Notifications.Application.DTOs;
using kiwiDeal.Notifications.Domain.Repositories;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Notifications.Application.Queries;

public class GetNotificationsQueryHandler(
    INotificationRepository notificationRepository)
    : IRequestHandler<GetNotificationsQuery, Result<NotificationsResponseDto>>
{
    private const int Limit = 20;

    public async Task<Result<NotificationsResponseDto>> Handle(
        GetNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        var notifications = await notificationRepository.GetRecentForUserAsync(
            request.UserId, Limit, cancellationToken);

        var unreadCount = await notificationRepository.GetUnreadCountAsync(
            request.UserId, cancellationToken);

        var dtos = notifications.Select(NotificationMapper.ToDto).ToList();

        return Result.Success(new NotificationsResponseDto
        {
            Items = dtos,
            UnreadCount = unreadCount
        });
    }
}
