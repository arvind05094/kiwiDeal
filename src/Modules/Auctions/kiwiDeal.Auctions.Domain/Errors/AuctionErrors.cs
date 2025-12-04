using kiwiDeal.SharedKernel.Results;

namespace kiwiDeal.Auctions.Domain.Errors;

public static class AuctionErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound($"Auction with id '{id}' was not found.");

    public static Error Forbidden() =>
        Error.Forbidden("You do not have permission to perform this action.");

    public static Error AlreadyClosed() =>
        Error.AuctionClosed("This auction has already been closed.");

    public static Error NotStarted() =>
        Error.AuctionNotStarted("This auction has not started yet.");

    public static Error BidTooLow(decimal minimum) =>
        Error.BidTooLow($"Bid must be greater than the current highest bid of {minimum:C}.");

    public static Error BidderIsSeller() =>
        Error.BidderIsSeller("You cannot bid on your own listing.");
}
