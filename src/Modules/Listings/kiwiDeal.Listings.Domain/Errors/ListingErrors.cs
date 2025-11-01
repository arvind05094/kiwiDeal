using kiwiDeal.SharedKernel.Results;

namespace kiwiDeal.Listings.Domain.Errors;

public static class ListingErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound($"Listings.NotFound", $"Listing with id '{id}' was not found.");

    public static Error Forbidden() =>
        Error.Forbidden("Listings.Forbidden", "You do not have permission to perform this action.");

    public static Error AlreadyClosed() =>
        Error.Conflict("Listings.AlreadyClosed", "This listing has already been closed.");
}
