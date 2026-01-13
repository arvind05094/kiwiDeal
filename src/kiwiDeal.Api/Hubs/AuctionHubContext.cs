using kiwiDeal.Auctions.Application.Commands;
using Microsoft.AspNetCore.SignalR;

namespace kiwiDeal.Api.Hubs;

public sealed class AuctionHubContext(IHubContext<AuctionHub> hubContext) : IAuctionHubContext
{
    public async Task SendBidPlaced(
        string auctionId,
        Guid bidId,
        Guid bidderId,
        decimal amount,
        DateTimeOffset newEndTime,
        CancellationToken cancellationToken = default)
    {
        await hubContext.Clients.Group(auctionId).SendAsync(
            "BidPlaced",
            new
            {
                AuctionId = auctionId,
                BidId = bidId,
                BidderId = bidderId,
                Amount = amount,
                NewEndTime = newEndTime
            },
            cancellationToken);
    }

    public async Task SendAuctionClosed(
    string auctionId,
    Guid? winnerId,
    decimal? finalAmount,
    CancellationToken cancellationToken = default)
    {
        await hubContext.Clients.Group(auctionId).SendAsync(
            "AuctionClosed",
            new
            {
                AuctionId = auctionId,
                WinnerId = winnerId,
                FinalAmount = finalAmount
            },
            cancellationToken);
    }
}
