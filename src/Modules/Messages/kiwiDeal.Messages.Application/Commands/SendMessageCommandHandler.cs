using kiwiDeal.Messages.Application.DTOs;
using kiwiDeal.Messages.Domain.Entities;
using kiwiDeal.Messages.Domain.Errors;
using kiwiDeal.Messages.Domain.Repositories;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Messages.Application.Commands;

public class SendMessageCommandHandler(
    IConversationRepository conversationRepository,
    IMessagesUnitOfWork unitOfWork,
    IMessageHubContext hubContext) : IRequestHandler<SendMessageCommand, Result<MessageDto>>
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
        conversation.TouchUpdatedAt();
        conversationRepository.Update(conversation);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await hubContext.SendMessageReceived(
            conversation.Id.Value,
            message.Id.Value,
            message.SenderId,
            message.SenderName,
            message.Content,
            message.CreatedAt,
            cancellationToken);

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
