using kiwiDeal.Notifications.Domain.Entities;
using kiwiDeal.Notifications.Domain.Repositories;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Notifications.Application.Commands;

public class MarkNotificationAsReadCommandHandler(
    INotificationRepository notificationRepository,
    INotificationsUnitOfWork unitOfWork)
    : IRequestHandler<MarkNotificationAsReadCommand, Result>
{
    public async Task<Result> Handle(
        MarkNotificationAsReadCommand request,
        CancellationToken cancellationToken)
    {
        var notification = await notificationRepository.GetByIdAsync(
            NotificationId.From(request.NotificationId), cancellationToken);

        if (notification is null || notification.RecipientId != request.UserId)
            return Result.Failure(Error.NotFound("Notification was not found."));

        notification.MarkAsRead();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
