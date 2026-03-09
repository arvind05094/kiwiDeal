using kiwiDeal.Auctions.Application.Commands;
using kiwiDeal.Auctions.Domain.Entities;
using kiwiDeal.Auctions.Domain.Repositories;
using kiwiDeal.SharedKernel.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace kiwiDeal.Auctions.Application.Events;

public sealed class ListingCancelledEventHandler(
    IAuctionRepository auctionRepository,
    IAuctionsUnitOfWork unitOfWork,
    IAuctionHubContext hubContext,
    ILogger<ListingCancelledEventHandler> logger) : INotificationHandler<ListingCancelledEvent>
{
    public async Task Handle(ListingCancelledEvent notification, CancellationToken cancellationToken)
    {
        var auction = await auctionRepository.GetByListingIdAsync(notification.ListingId, cancellationToken);

        if (auction is null)
            return; // not an auction listing

        if (auction.Status == AuctionStatus.Closed)
            return; // already closed

        var result = auction.Close();
        if (result.IsFailure)
        {
            logger.LogWarning(
                "Failed to close auction {AuctionId} after listing {ListingId} was cancelled: {Error}",
                auction.Id, notification.ListingId, result.Error.Message);
            return;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Auction {AuctionId} closed after listing {ListingId} was cancelled",
            auction.Id, notification.ListingId);

        await hubContext.SendAuctionClosed(
            auction.Id.Value.ToString(),
            auction.CurrentHighestBidderId,
            auction.CurrentHighestBid,
            cancellationToken);
    }
}