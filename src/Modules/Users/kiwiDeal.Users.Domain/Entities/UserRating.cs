using kiwiDeal.SharedKernel.Entities;

namespace kiwiDeal.Users.Domain.Entities;

public sealed class UserRating : BaseEntity
{
    public UserRatingId Id { get; private set; } = null!;
    public UserId RaterId { get; private set; } = null!;
    public UserId RateeId { get; private set; } = null!;
    public int Stars { get; private set; }
    public string? Comment { get; private set; }

    private UserRating() { }

    public static UserRating Create(
        UserId raterId,
        UserId rateeId,
        int stars,
        string? comment)
    {
        return new UserRating
        {
            Id = UserRatingId.New(),
            RaterId = raterId,
            RateeId = rateeId,
            Stars = stars,
            Comment = comment?.Trim(),
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }
}
