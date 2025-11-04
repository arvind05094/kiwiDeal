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
}
