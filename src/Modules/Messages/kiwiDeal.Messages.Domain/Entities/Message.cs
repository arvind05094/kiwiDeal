using kiwiDeal.SharedKernel.Entities;
using kiwiDeal.SharedKernel.Results;
using kiwiDeal.SharedKernel.Interfaces;
using kiwiDeal.Messages.Domain.Errors;
namespace kiwiDeal.Messages.Domain.Entities;

public class Message : BaseEntity, ISoftDeletable
{
    private Message() { }

    public MessageId Id { get; private set; } = default!;
    public ConversationId ConversationId { get; private set; } = default!;
    public Guid SenderId { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public new DateTimeOffset CreatedAt { get; private set; }

    public static Result<Message> Create(
        ConversationId conversationId,
        Guid senderId,
        string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return Result.Failure<Message>(MessageErrors.ContentEmpty);

        var message = new Message
        {
            Id = MessageId.New(),
            ConversationId = conversationId,
            SenderId = senderId,
            Content = content.Trim(),
            CreatedAt = DateTimeOffset.UtcNow
        };

        return Result.Success(message);
    }
}
