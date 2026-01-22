using kiwiDeal.Auctions.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace kiwiDeal.Notifications.Application.Events;

internal sealed class AuctionClosedEventHandler(
    ILogger<AuctionClosedEventHandler> logger)
    : INotificationHandler<AuctionClosedEvent>
{
    public Task Handle(AuctionClosedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Notification received: AuctionClosedEvent for auction {AuctionId}, winner {WinnerId}",
            notification.AuctionId,
            notification.WinningBidderId?.ToString() ?? "none");

        return Task.CompletedTask;
    }
}
