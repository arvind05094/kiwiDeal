namespace kiwiDeal.Users.Api.Requests;

public sealed record LoginRequest(
    string Email,
    string Password);
