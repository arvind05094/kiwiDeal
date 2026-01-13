using kiwiDeal.SharedKernel.Entities;
using kiwiDeal.SharedKernel.Interfaces;
using kiwiDeal.SharedKernel.Results;
using kiwiDeal.Users.Domain.Enums;
using kiwiDeal.Users.Domain.Errors;

namespace kiwiDeal.Users.Domain.Entities;

public sealed class User : AggregateRoot, ISoftDeletable
{
    private readonly List<RefreshToken> _refreshTokens = [];

    public UserId Id { get; private set; } = null!;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;

    public Region Region { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }

    public IReadOnlyList<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    private User() { }

    public static Result<User> Create(
        string email,
        string passwordHash,
        string firstName,
        string lastName,
        Region region)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Result.Failure<User>(Error.ValidationFailed("Email is required."));

        if (string.IsNullOrWhiteSpace(passwordHash))
            return Result.Failure<User>(Error.ValidationFailed("Password hash is required."));

        if (string.IsNullOrWhiteSpace(firstName))
            return Result.Failure<User>(Error.ValidationFailed("First name is required."));

        if (string.IsNullOrWhiteSpace(lastName))
            return Result.Failure<User>(Error.ValidationFailed("Last name is required."));

        var user = new User
        {
            Id = UserId.New(),
            Email = email.ToLowerInvariant().Trim(),
            PasswordHash = passwordHash,
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            Region = region,
        };

        return Result.Success(user);
    }

    public RefreshToken AddRefreshToken(string token, DateTimeOffset expiresAt)
    {
        RevokeAllRefreshTokens();

        var refreshToken = RefreshToken.Create(Id, token, expiresAt);
        _refreshTokens.Add(refreshToken);
        UpdatedAt = DateTimeOffset.UtcNow;

        return refreshToken;
    }

    public void UpdateProfile(string firstName, string lastName, Region region)
    {
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Region = region;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public Result<RefreshToken> GetActiveRefreshToken(string token)
    {
        var refreshToken = _refreshTokens.SingleOrDefault(r => r.Token == token);

        if (refreshToken is null || !refreshToken.IsActive)
            return Result.Failure<RefreshToken>(UserErrors.InvalidRefreshToken);

        return Result.Success(refreshToken);
    }

    public void Delete()
    {
        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
        RevokeAllRefreshTokens();
    }

    private void RevokeAllRefreshTokens()
    {
        foreach (var token in _refreshTokens.Where(t => t.IsActive))
            token.Revoke();
    }
}
