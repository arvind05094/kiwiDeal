using kiwiDeal.SharedKernel.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace kiwiDeal.Notifications.Application.Events;

internal sealed class ListingCreatedEventHandler(
    ILogger<ListingCreatedEventHandler> logger)
    : INotificationHandler<ListingCreatedEvent>
{
    public Task Handle(ListingCreatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Notification received: ListingCreatedEvent for listing {ListingId}",
            notification.ListingId);

        return Task.CompletedTask;
    }
}
