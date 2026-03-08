using kiwiDeal.Messages.Application.Commands;
using Microsoft.AspNetCore.SignalR;

namespace kiwiDeal.Messages.Api;

public class MessageHubContext(IHubContext<MessageHub> hubContext) : IMessageHubContext
{
    public async Task SendMessageReceived(
        Guid conversationId,
        Guid messageId,
        Guid senderId,
        string senderName,
        string content,
        DateTimeOffset createdAt,
        CancellationToken cancellationToken = default)
    {
        await hubContext.Clients
            .Group(conversationId.ToString())
            .SendAsync("MessageReceived", new
            {
                conversationId,
                messageId,
                senderId,
                senderName,
                content,
                createdAt
            }, cancellationToken);
    }

    public async Task SendConversationUpdated(
        Guid recipientUserId,
        Guid conversationId,
        string lastMessagePreview,
        DateTimeOffset updatedAt,
        CancellationToken cancellationToken = default)
    {
        await hubContext.Clients
            .User(recipientUserId.ToString())
            .SendAsync("ConversationUpdated", new
            {
                conversationId,
                lastMessagePreview,
                updatedAt
            }, cancellationToken);
    }
}