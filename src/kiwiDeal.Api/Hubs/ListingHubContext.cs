using kiwiDeal.Listings.Application.Events;
using Microsoft.AspNetCore.SignalR;

namespace kiwiDeal.Api.Hubs;

public sealed class ListingHubContext(IHubContext<ListingHub> hubContext) : IListingHubContext
{
    public async Task SendListingSold(string listingId, CancellationToken cancellationToken = default)
    {
        await hubContext.Clients.Group(listingId).SendAsync(
            "ListingSold",
            new { ListingId = listingId },
            cancellationToken);
    }
}
