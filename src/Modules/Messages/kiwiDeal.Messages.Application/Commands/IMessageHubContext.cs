namespace kiwiDeal.Messages.Application.Commands;

public interface IMessageHubContext
{
    Task SendMessageReceived(
        Guid conversationId,
        Guid messageId,
        Guid senderId,
        string senderName,
        string content,
        DateTimeOffset createdAt,
        CancellationToken cancellationToken = default);

    Task SendConversationUpdated(
        Guid recipientUserId,
        Guid conversationId,
        string lastMessagePreview,
        DateTimeOffset updatedAt,
        CancellationToken cancellationToken = default);
}