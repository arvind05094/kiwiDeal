using kiwiDeal.SharedKernel.Entities;
using kiwiDeal.SharedKernel.Results;
using kiwiDeal.SharedKernel.Interfaces;
using kiwiDeal.Messages.Domain.Errors;
namespace kiwiDeal.Messages.Domain.Entities;

public class Conversation : AggregateRoot, ISoftDeletable
{
    private Conversation() { }

    public ConversationId Id { get; private set; } = default!;
    public Guid ListingId { get; private set; }
    public Guid SenderId { get; private set; }
    public Guid RecipientId { get; private set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    public new DateTimeOffset CreatedAt { get; private set; }
    public new DateTimeOffset UpdatedAt { get; private set; }

    private readonly List<Message> _messages = new();
    public IReadOnlyCollection<Message> Messages => _messages.AsReadOnly();

    public static Result<Conversation> Create(
        Guid listingId,
        Guid senderId,
        Guid recipientId)
    {
        if (senderId == recipientId)
            return Result.Failure<Conversation>(MessageErrors.CannotMessageSelf);

        var conversation = new Conversation
        {
            Id = ConversationId.New(),
            ListingId = listingId,
            SenderId = senderId,
            RecipientId = recipientId,
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
