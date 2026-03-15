using kiwiDeal.Notifications.Application.DTOs;
using kiwiDeal.Notifications.Domain.Entities;
using kiwiDeal.Notifications.Domain.Repositories;
using kiwiDeal.SharedKernel.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace kiwiDeal.Notifications.Application.Events;

internal sealed class AuctionClosedEventHandler(
    INotificationRepository notificationRepository,
    INotificationsUnitOfWork unitOfWork,
    INotificationHubContext hubContext,
    ILogger<AuctionClosedEventHandler> logger)
    : INotificationHandler<AuctionClosedEvent>
{
    public async Task Handle(AuctionClosedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Notification received: AuctionClosedEvent for auction {AuctionId}, winner {WinnerId}",
            notification.AuctionId,
            notification.WinningBidderId?.ToString() ?? "none");

        if (notification.WinningBidderId is null)
            return;

        var won = Notification.Create(
            notification.WinningBidderId.Value,
            NotificationType.AuctionWon,
            "Auction won!",
            "Congratulations, you won the auction.",
            $"/auctions/{notification.AuctionId}");

        notificationRepository.Add(won);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await hubContext.SendNotificationReceived(
            won.RecipientId, NotificationMapper.ToDto(won), cancellationToken);
    }
}
