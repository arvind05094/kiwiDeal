using kiwiDeal.SharedKernel.Entities;
using kiwiDeal.SharedKernel.Results;
using kiwiDeal.SharedKernel.Interfaces;
using kiwiDeal.Messages.Domain.Errors;

namespace kiwiDeal.Messages.Domain.Entities;

public class Conversation : AggregateRoot, ISoftDeletable
{
    private Conversation() { }

    public ConversationId Id { get; private set; } = default!;
    public Guid SenderId { get; private set; }
    public string SenderName { get; private set; } = string.Empty;
    public Guid RecipientId { get; private set; }
    public string RecipientName { get; private set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    public new DateTimeOffset CreatedAt { get; private set; }
    public new DateTimeOffset UpdatedAt { get; private set; }

    private readonly List<Message> _messages = new();
    public IReadOnlyCollection<Message> Messages => _messages.AsReadOnly();

    public static Result<Conversation> Create(
        Guid senderId,
        string senderName,
        Guid recipientId,
        string recipientName)
    {
        if (senderId == recipientId)
            return Result.Failure<Conversation>(MessageErrors.CannotMessageSelf);

        var conversation = new Conversation
        {
            Id = ConversationId.New(),
            SenderId = senderId,
            SenderName = senderName,
            RecipientId = recipientId,
            RecipientName = recipientName,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        return Result.Success(conversation);
    }

    public bool IsParticipant(Guid userId) =>
        SenderId == userId || RecipientId == userId;

    public void TouchUpdatedAt() =>
        UpdatedAt = DateTimeOffset.UtcNow;

    public Result AddMessage(Message message)
    {
        _messages.Add(message);
        TouchUpdatedAt();

        return Result.Success();
    }
}