namespace kiwiDeal.Messages.Domain.Entities;

public record MessageId
{
    public Guid Value { get; }
    private MessageId(Guid value) { Value = value; }
    public static MessageId New() => new(Guid.CreateVersion7());
    public static MessageId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}