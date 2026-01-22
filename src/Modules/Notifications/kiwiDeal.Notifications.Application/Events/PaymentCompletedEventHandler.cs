using kiwiDeal.Payments.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace kiwiDeal.Notifications.Application.Events;

internal sealed class PaymentCompletedEventHandler(
    ILogger<PaymentCompletedEventHandler> logger)
    : INotificationHandler<PaymentCompletedEvent>
{
    public Task Handle(PaymentCompletedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Notification received: PaymentCompletedEvent for payment {PaymentId}, buyer {BuyerId}",
            notification.PaymentId,
            notification.BuyerId);

        return Task.CompletedTask;
    }
}
