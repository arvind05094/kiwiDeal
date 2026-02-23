using kiwiDeal.SharedKernel.Results;

namespace kiwiDeal.Auctions.Domain.Errors;

public static class AuctionWatchlistErrors
{
    public static Error CannotWatchOwnAuction() =>
        Error.Conflict("You cannot watch your own auction.");

    public static Error AuctionNotWatchable() =>
        Error.Conflict("You can only watch Active or Scheduled auctions.");

    public static Error AlreadyWatching() =>
        Error.Conflict("You are already watching this auction.");

    public static Error NotFound() =>
        Error.NotFound("Auction watchlist entry not found.");
}
