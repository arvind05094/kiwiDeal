using kiwiDeal.Notifications.Application.DTOs;
using kiwiDeal.Notifications.Domain.Entities;
using kiwiDeal.Notifications.Domain.Repositories;
using kiwiDeal.SharedKernel.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace kiwiDeal.Notifications.Application.Events;

internal sealed class MessageSentEventHandler(
    INotificationRepository notificationRepository,
    INotificationsUnitOfWork unitOfWork,
    INotificationHubContext hubContext,
    ILogger<MessageSentEventHandler> logger)
    : INotificationHandler<MessageSentEvent>
{
    public async Task Handle(MessageSentEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Notification received: MessageSentEvent for conversation {ConversationId}, recipient {RecipientId}",
            notification.ConversationId,
            notification.RecipientId);

        var message = Notification.Create(
            notification.RecipientId,
            NotificationType.NewMessage,
            "New message",
            $"{notification.SenderName}: {notification.Content}",
            $"/messages/{notification.ConversationId}");

        notificationRepository.Add(message);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await hubContext.SendNotificationReceived(
            message.RecipientId, NotificationMapper.ToDto(message), cancellationToken);
    }
}
