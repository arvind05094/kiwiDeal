namespace kiwiDeal.Users.Application.DTOs;

public sealed record AuthResponse(
    string AccessToken,
    UserResponse User);
