using kiwiDeal.Listings.Domain.Entities;
using kiwiDeal.Listings.Domain.Repositories;
using kiwiDeal.SharedKernel.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace kiwiDeal.Listings.Application.Events;

public sealed class PaymentCompletedEventHandler(
    IListingRepository listingRepository,
    IListingsUnitOfWork unitOfWork,
    IListingHubContext listingHubContext,
    ILogger<PaymentCompletedEventHandler> logger) : INotificationHandler<PaymentCompletedEvent>
{
    public async Task Handle(PaymentCompletedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.PaymentType != "FixedPrice")
            return;

        var listing = await listingRepository.GetByIdAsync(
            ListingId.From(notification.ListingId), cancellationToken);

        if (listing is null)
        {
            logger.LogWarning(
                "Listing {ListingId} not found when handling PaymentCompletedEvent {PaymentId}",
                notification.ListingId, notification.PaymentId);
            return;
        }

        var result = listing.MarkSold();
        if (result.IsFailure)
        {
            logger.LogWarning(
                "Failed to mark listing {ListingId} as Sold after payment {PaymentId}: {Error}",
                listing.Id, notification.PaymentId, result.Error.Message);
            return;
        }

        listingRepository.Update(listing);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await listingHubContext.SendListingSold(listing.Id.Value.ToString(), cancellationToken);
        logger.LogInformation(
            "Listing {ListingId} marked as Sold after fixed price payment {PaymentId} completed",
            listing.Id, notification.PaymentId);
    }
}
