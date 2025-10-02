namespace kiwiDeal.SharedKernel.Results;

public enum ErrorCode
{
    NotFound,
    Forbidden,
    Unauthorised,
    Conflict,
    ValidationFailed,
    Unexpected,
    AuctionClosed,
    AuctionNotStarted,
    BidTooLow,
    BidderIsSeller,
    ListingAlreadyClosed,
    PaymentAlreadyProcessed,
    InvalidRefreshToken
}
