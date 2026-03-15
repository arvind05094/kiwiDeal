using kiwiDeal.Notifications.Application.DTOs;
using kiwiDeal.Notifications.Domain.Entities;
using kiwiDeal.Notifications.Domain.Repositories;
using kiwiDeal.SharedKernel.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace kiwiDeal.Notifications.Application.Events;

internal sealed class PaymentCompletedEventHandler(
    INotificationRepository notificationRepository,
    INotificationsUnitOfWork unitOfWork,
    INotificationHubContext hubContext,
    ILogger<PaymentCompletedEventHandler> logger)
    : INotificationHandler<PaymentCompletedEvent>
{
    public async Task Handle(PaymentCompletedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Notification received: PaymentCompletedEvent for payment {PaymentId}, buyer {BuyerId}",
            notification.PaymentId,
            notification.BuyerId);

        var linkUrl = $"/listings/{notification.ListingId}";

        var purchased = Notification.Create(
            notification.BuyerId,
            NotificationType.ItemPurchased,
            "Purchase complete",
            "Your purchase was successful.",
            linkUrl);

        var sold = Notification.Create(
            notification.SellerId,
            NotificationType.ItemSold,
            "Item sold!",
            "One of your items has been sold.",
            linkUrl);

        notificationRepository.Add(purchased);
        notificationRepository.Add(sold);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await hubContext.SendNotificationReceived(
            purchased.RecipientId, NotificationMapper.ToDto(purchased), cancellationToken);
        await hubContext.SendNotificationReceived(
            sold.RecipientId, NotificationMapper.ToDto(sold), cancellationToken);
    }
}
