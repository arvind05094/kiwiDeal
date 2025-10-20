using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace kiwiDeal.SharedKernel.Results;

public static class ResultExtensions
{
    public static IActionResult ToProblemDetails(this Error error)
    {
        var statusCode = error.Code switch
        {
            ErrorCode.NotFound => StatusCodes.Status404NotFound,
            ErrorCode.Forbidden => StatusCodes.Status403Forbidden,
            ErrorCode.Unauthorised => StatusCodes.Status401Unauthorized,
            ErrorCode.Conflict => StatusCodes.Status409Conflict,
            ErrorCode.ValidationFailed => StatusCodes.Status400BadRequest,
            ErrorCode.AuctionClosed => StatusCodes.Status409Conflict,
            ErrorCode.AuctionNotStarted => StatusCodes.Status409Conflict,
            ErrorCode.BidTooLow => StatusCodes.Status400BadRequest,
            ErrorCode.BidderIsSeller => StatusCodes.Status400BadRequest,
            ErrorCode.ListingAlreadyClosed => StatusCodes.Status409Conflict,
            ErrorCode.PaymentAlreadyProcessed => StatusCodes.Status409Conflict,
            ErrorCode.InvalidRefreshToken => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = error.Code.ToString(),
            Detail = error.Message
        };

        return new ObjectResult(problemDetails) { StatusCode = statusCode };
    }
}
