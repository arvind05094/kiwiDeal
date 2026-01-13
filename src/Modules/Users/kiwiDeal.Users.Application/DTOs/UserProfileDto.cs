namespace kiwiDeal.Users.Application.DTOs;

public sealed record UserProfileDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Region,
    DateTimeOffset MemberSince,
    double? AverageRating,
    int TotalRatings);
