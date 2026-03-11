namespace kiwiDeal.Users.Application.DTOs;

public sealed record UserRatingDto(
    int Stars,
    string? Comment,
    string RaterName,
    DateTimeOffset CreatedAt);