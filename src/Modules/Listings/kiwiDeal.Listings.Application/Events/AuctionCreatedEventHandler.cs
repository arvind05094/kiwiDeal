using kiwiDeal.SharedKernel.Contracts;
using kiwiDeal.Listings.Domain.Entities;
using kiwiDeal.Listings.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace kiwiDeal.Listings.Application.Events;

public sealed class AuctionCreatedEventHandler(
    IListingRepository listingRepository,
    IListingsUnitOfWork unitOfWork,
    ILogger<AuctionCreatedEventHandler> logger) : INotificationHandler<AuctionCreatedEvent>
{
    public async Task Handle(AuctionCreatedEvent notification, CancellationToken cancellationToken)
    {
        var listing = await listingRepository.GetByIdAsync(
            ListingId.From(notification.ListingId), cancellationToken);

        if (listing is null)
        {
            logger.LogWarning(
                "Listing {ListingId} not found when handling AuctionCreatedEvent for auction {AuctionId}",
                notification.ListingId, notification.AuctionId);
            return;
        }

        listing.AssignAuction(notification.AuctionId);
        listingRepository.Update(listing);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Listing {ListingId} updated with AuctionId {AuctionId}",
            listing.Id, notification.AuctionId);
    }
}
