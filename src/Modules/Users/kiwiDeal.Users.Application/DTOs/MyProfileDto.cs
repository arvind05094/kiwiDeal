namespace kiwiDeal.Users.Application.DTOs;

public sealed record MyProfileDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string Region,
    DateTimeOffset MemberSince);
