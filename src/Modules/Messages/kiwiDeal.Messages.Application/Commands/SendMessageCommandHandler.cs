using kiwiDeal.Messages.Application.DTOs;
using kiwiDeal.Messages.Domain.Entities;
using kiwiDeal.Messages.Domain.Errors;
using kiwiDeal.Messages.Domain.Repositories;
using kiwiDeal.SharedKernel.Contracts;
using kiwiDeal.SharedKernel.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace kiwiDeal.Messages.Application.Commands;

public class SendMessageCommandHandler(
    IConversationRepository conversationRepository,
    IMessagesUnitOfWork unitOfWork,
    IMessageHubContext hubContext,
    IPublisher publisher,
    ILogger<SendMessageCommandHandler> logger) : IRequestHandler<SendMessageCommand, Result<MessageDto>>
{
    public async Task<Result<MessageDto>> Handle(
        SendMessageCommand request,
        CancellationToken cancellationToken)
    {
        var conversation = await conversationRepository.GetByIdAsync(
            ConversationId.From(request.ConversationId), cancellationToken);

        if (conversation is null)
            return Result.Failure<MessageDto>(MessageErrors.ConversationNotFound(request.ConversationId));

        if (!conversation.IsParticipant(request.SenderId))
            return Result.Failure<MessageDto>(MessageErrors.NotParticipant);

        var messageResult = Message.Create(
            conversation.Id,
            request.SenderId,
            request.SenderName,
            request.Content);

        if (messageResult.IsFailure)
            return Result.Failure<MessageDto>(messageResult.Error);

        var message = messageResult.Value;
        var addResult = conversation.AddMessage(message);
        if (addResult.IsFailure)
            return Result.Failure<MessageDto>(addResult.Error);
        conversation.TouchUpdatedAt();
        conversationRepository.Update(conversation);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var recipientId = conversation.SenderId == request.SenderId
            ? conversation.RecipientId
            : conversation.SenderId;

        try
        {
            await hubContext.SendMessageReceived(
                conversation.Id.Value,
                message.Id.Value,
                message.SenderId,
                message.SenderName,
                message.Content,
                message.CreatedAt,
                cancellationToken);

            await hubContext.SendConversationUpdated(
                recipientId,
                conversation.Id.Value,
                message.Content,
                message.CreatedAt,
                cancellationToken);

            await publisher.Publish(new MessageSentEvent(
                conversation.Id.Value,
                recipientId,
                message.SenderId,
                message.SenderName,
                message.Content), cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to push live updates for message {MessageId} in conversation {ConversationId}", message.Id.Value, conversation.Id.Value);
        }

        return Result.Success(new MessageDto
        {
            Id = message.Id.Value,
            ConversationId = conversation.Id.Value,
            SenderId = message.SenderId,
            SenderName = message.SenderName,
            Content = message.Content,
            CreatedAt = message.CreatedAt
        });
    }
}
