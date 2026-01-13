using kiwiDeal.Users.Domain.Enums;

namespace kiwiDeal.Users.Api.Requests;

public sealed record UpdateProfileRequest(
    string FirstName,
    string LastName,
    Region Region);
