using kiwiDeal.SharedKernel.Results;

namespace kiwiDeal.Listings.Domain.Errors;

public static class ListingErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound($"Listing with id '{id}' was not found.");

    public static Error Forbidden() =>
        Error.Forbidden("You do not have permission to perform this action.");

    public static Error AlreadyClosed() =>
        Error.Conflict("This listing has already been closed.");

    public static Error TooManyImages() =>
        Error.Conflict("A listing cannot have more than 3 images.");

    public static Error NotOwner() =>
        Error.Forbidden("You do not own this listing.");
    public static Error NotActive() =>
        Error.Conflict("This listing is not active.");

    public static Error CannotWatchOwnListing() =>
        Error.Conflict("You cannot watch your own listing.");

    public static Error AlreadyWatching() =>
        Error.Conflict("You are already watching this listing.");

    public static Error WatchlistEntryNotFound() =>
        Error.NotFound("Watchlist entry not found.");
}
