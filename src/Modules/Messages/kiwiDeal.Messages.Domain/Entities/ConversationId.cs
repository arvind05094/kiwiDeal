namespace kiwiDeal.Messages.Domain.Entities;

public record ConversationId
{
    public Guid Value { get; }
    private ConversationId(Guid value) { Value = value; }
    public static ConversationId New() => new(Guid.CreateVersion7());
    public static ConversationId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}