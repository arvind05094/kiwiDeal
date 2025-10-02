namespace kiwiDeal.SharedKernel.Results;

public sealed record Error(ErrorCode Code, string Message)
{
    public static readonly Error None = new(ErrorCode.Unexpected, string.Empty);

    public static Error NotFound(string message) => new(ErrorCode.NotFound, message);
    public static Error Forbidden(string message) => new(ErrorCode.Forbidden, message);
    public static Error Unauthorised(string message) => new(ErrorCode.Unauthorised, message);
    public static Error Conflict(string message) => new(ErrorCode.Conflict, message);
    public static Error ValidationFailed(string message) => new(ErrorCode.ValidationFailed, message);
    public static Error Unexpected(string message) => new(ErrorCode.Unexpected, message);
}
