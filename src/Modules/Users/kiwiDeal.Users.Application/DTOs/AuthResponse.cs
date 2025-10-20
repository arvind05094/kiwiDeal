namespace kiwiDeal.Users.Application.DTOs;

public sealed record AuthResponse(
    string AccessToken,
    string RefreshToken,
    UserResponse User);
