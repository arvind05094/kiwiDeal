using kiwiDeal.Payments.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace kiwiDeal.Users.Application.Events;

public sealed class PaymentCompletedEventHandler(
    ILogger<PaymentCompletedEventHandler> logger) : INotificationHandler<PaymentCompletedEvent>
{
    public Task Handle(PaymentCompletedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "PaymentCompletedEvent received for buyer {BuyerId} and seller {SellerId}. Rating prompt available.",
            notification.BuyerId,
            notification.SellerId);
        return Task.CompletedTask;
    }
}
