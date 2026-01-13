using kiwiDeal.Auctions.Domain.Events;
using kiwiDeal.Payments.Domain.Entities;
using kiwiDeal.Payments.Domain.Repositories;
using kiwiDeal.SharedKernel.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
namespace kiwiDeal.Payments.Application.Events;
public sealed class AuctionClosedEventHandler(
    IPaymentRepository paymentRepository,
    IPaymentsUnitOfWork unitOfWork,
    ILogger<AuctionClosedEventHandler> logger) : INotificationHandler<AuctionClosedEvent>
{
    public async Task Handle(AuctionClosedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.WinningBidderId is null || notification.WinningAmount is null)
        {
            logger.LogInformation(
                "Auction {AuctionId} closed with no winner — no payment initiated",
                notification.AuctionId);
            return;
        }
        var existing = await paymentRepository.GetByAuctionIdAsync(
            notification.AuctionId, cancellationToken);
        if (existing is not null)
        {
            logger.LogWarning(
                "Payment for auction {AuctionId} already exists — skipping",
                notification.AuctionId);
            return;
        }
        var result = Payment.Create(
            notification.AuctionId,
            notification.ListingId,
            notification.WinningBidderId.Value,
            notification.SellerId,
            notification.WinningAmount.Value,
            "Auction");
        if (result.IsFailure)
        {
            logger.LogWarning(
                "Failed to create payment for auction {AuctionId}: {Error}",
                notification.AuctionId, result.Error.Message);
            return;
        }
        paymentRepository.Add(result.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation(
            "Payment {PaymentId} created for auction {AuctionId}",
            result.Value.Id, notification.AuctionId);
    }
}
