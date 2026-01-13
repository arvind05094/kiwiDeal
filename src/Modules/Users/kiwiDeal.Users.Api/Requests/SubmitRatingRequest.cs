namespace kiwiDeal.Users.Api.Requests;

public sealed record SubmitRatingRequest(
    int Stars,
    string? Comment);
