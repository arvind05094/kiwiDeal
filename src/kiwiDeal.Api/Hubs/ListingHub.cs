using Microsoft.AspNetCore.SignalR;

namespace kiwiDeal.Api.Hubs;

public sealed class ListingHub : Hub
{
    public async Task JoinListing(string listingId) =>
        await Groups.AddToGroupAsync(Context.ConnectionId, listingId);

    public async Task LeaveListing(string listingId) =>
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, listingId);
}
