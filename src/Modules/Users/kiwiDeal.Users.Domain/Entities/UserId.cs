namespace kiwiDeal.Users.Domain.Entities;

public record UserId
{
    public Guid Value { get; }
    private UserId(Guid value) { Value = value; }
    public static UserId New() => new(Guid.CreateVersion7());
    public static UserId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
