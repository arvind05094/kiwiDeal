namespace kiwiDeal.Users.Domain.Entities;

public record UserRatingId
{
    public Guid Value { get; }
    private UserRatingId(Guid value) { Value = value; }
    public static UserRatingId New() => new(Guid.CreateVersion7());
    public static UserRatingId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
