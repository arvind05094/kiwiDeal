using kiwiDeal.Notifications.Domain.Repositories;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Notifications.Application.Commands;

public class MarkAllNotificationsAsReadCommandHandler(
    INotificationRepository notificationRepository,
    INotificationsUnitOfWork unitOfWork)
    : IRequestHandler<MarkAllNotificationsAsReadCommand, Result>
{
    public async Task<Result> Handle(
        MarkAllNotificationsAsReadCommand request,
        CancellationToken cancellationToken)
    {
        var unread = await notificationRepository.GetUnreadForUserAsync(
            request.UserId, cancellationToken);

        foreach (var notification in unread)
            notification.MarkAsRead();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
