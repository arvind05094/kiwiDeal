using kiwiDeal.Auctions.Domain.Events;
using kiwiDeal.Listings.Domain.Entities;
using kiwiDeal.Listings.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace kiwiDeal.Listings.Application.Events;

public sealed class AuctionClosedEventHandler(
    IListingRepository listingRepository,
    IListingsUnitOfWork unitOfWork,
    ILogger<AuctionClosedEventHandler> logger) : INotificationHandler<AuctionClosedEvent>
{
    public async Task Handle(AuctionClosedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.WinningBidderId is null)
        {
            logger.LogInformation(
                "Auction {AuctionId} closed with no winner — listing remains Active",
                notification.AuctionId);
            return;
        }

        var listing = await listingRepository.GetByIdAsync(
            ListingId.From(notification.ListingId), cancellationToken);
        if (listing is null)
        {
            logger.LogWarning(
                "Listing for auction {AuctionId} not found — cannot mark as Sold",
                notification.AuctionId);
            return;
        }

        var result = listing.MarkSold();
        if (result.IsFailure)
        {
            logger.LogWarning(
                "Failed to mark listing {ListingId} as Sold: {Error}",
                listing.Id, result.Error.Message);
            return;
        }

        listingRepository.Update(listing);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Listing {ListingId} marked as Sold after auction {AuctionId} closed",
            listing.Id, notification.AuctionId);
    }
}
