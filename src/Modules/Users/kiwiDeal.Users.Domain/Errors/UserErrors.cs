using kiwiDeal.SharedKernel.Results;

namespace kiwiDeal.Users.Domain.Errors;

public static class UserErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound($"User with ID '{id}' was not found.");

    public static readonly Error EmailAlreadyInUse =
        Error.Conflict("A user with this email address already exists.");

    public static readonly Error InvalidCredentials =
        Error.Unauthorised("The email or password is incorrect.");

    public static readonly Error InvalidRefreshToken =
        new(SharedKernel.Results.ErrorCode.InvalidRefreshToken, "The refresh token is invalid or has expired.");
    public static readonly Error AlreadyRated =
        Error.Conflict("You have already submitted a rating for this user.");

    public static Error RatingNotFound(Guid id) =>
        Error.NotFound($"Rating with ID '{id}' was not found.");
}
