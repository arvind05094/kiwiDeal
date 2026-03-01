namespace kiwiDeal.Users.Domain.Entities;

using kiwiDeal.SharedKernel.Interfaces;

public record UserRatingId : IStronglyTypedId
{
    public Guid Value { get; }
    private UserRatingId(Guid value) { Value = value; }
    public static UserRatingId New() => new(Guid.CreateVersion7());
    public static UserRatingId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
