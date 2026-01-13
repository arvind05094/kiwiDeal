using kiwiDeal.Auctions.Domain.Events;
using kiwiDeal.Listings.Domain.Entities;
using kiwiDeal.Listings.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using kiwiDeal.SharedKernel.Results;

namespace kiwiDeal.Listings.Application.Events;

public sealed class AuctionClosedEventHandler(
    IListingRepository listingRepository,
    IListingsUnitOfWork unitOfWork,
    ILogger<AuctionClosedEventHandler> logger) : INotificationHandler<AuctionClosedEvent>
{
    public async Task Handle(AuctionClosedEvent notification, CancellationToken cancellationToken)
    {
        var listing = await listingRepository.GetByIdAsync(
            ListingId.From(notification.ListingId), cancellationToken);

        if (listing is null)
        {
            logger.LogWarning(
                "Listing {ListingId} not found when handling AuctionClosedEvent for auction {AuctionId}",
                notification.ListingId, notification.AuctionId);
            return;
        }

        Result result;

        if (notification.WinningBidderId is null)
        {
            result = listing.Cancel();
            if (result.IsFailure)
            {
                logger.LogWarning(
                    "Failed to cancel listing {ListingId} after auction {AuctionId} closed with no bids: {Error}",
                    listing.Id, notification.AuctionId, result.Error.Message);
                return;
            }

            logger.LogInformation(
                "Listing {ListingId} cancelled after auction {AuctionId} closed with no bids",
                listing.Id, notification.AuctionId);
        }
        else
        {
            result = listing.MarkSold();
            if (result.IsFailure)
            {
                logger.LogWarning(
                    "Failed to mark listing {ListingId} as Sold after auction {AuctionId} closed: {Error}",
                    listing.Id, notification.AuctionId, result.Error.Message);
                return;
            }

            logger.LogInformation(
                "Listing {ListingId} marked as Sold after auction {AuctionId} closed with winner {WinnerId}",
                listing.Id, notification.AuctionId, notification.WinningBidderId);
        }

        listingRepository.Update(listing);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
