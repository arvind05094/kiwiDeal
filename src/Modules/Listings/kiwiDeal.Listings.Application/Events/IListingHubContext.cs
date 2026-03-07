namespace kiwiDeal.Listings.Application.Events;

public interface IListingHubContext
{
    Task SendListingSold(string listingId, CancellationToken cancellationToken = default);
}
