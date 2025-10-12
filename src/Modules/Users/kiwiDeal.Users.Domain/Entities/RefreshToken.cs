using kiwiDeal.SharedKernel.Entities;

namespace kiwiDeal.Users.Domain.Entities;

public sealed class RefreshToken : BaseEntity
{
    public Guid Id { get; private set; }
    public UserId UserId { get; private set; } = null!;
    public string Token { get; private set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }

    private RefreshToken() { }

    public static RefreshToken Create(UserId userId, string token, DateTimeOffset expiresAt)
    {
        return new RefreshToken
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            Token = token,
            ExpiresAt = expiresAt,
            IsRevoked = false,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }

    public bool IsExpired => DateTimeOffset.UtcNow > ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;

    public void Revoke()
    {
        IsRevoked = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
