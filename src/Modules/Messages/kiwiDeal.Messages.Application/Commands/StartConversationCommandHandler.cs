using kiwiDeal.Messages.Application.DTOs;
using kiwiDeal.Messages.Domain.Entities;
using kiwiDeal.Messages.Domain.Repositories;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Messages.Application.Commands;

public class StartConversationCommandHandler(
    IConversationRepository conversationRepository,
    IMessagesUnitOfWork unitOfWork) : IRequestHandler<StartConversationCommand, Result<ConversationDto>>
{
    public async Task<Result<ConversationDto>> Handle(
        StartConversationCommand request,
        CancellationToken cancellationToken)
    {
        var existing = await conversationRepository.GetByParticipantsAsync(
            request.SenderId, request.RecipientId, cancellationToken);

        if (existing is not null)
            return Result.Success(new ConversationDto
            {
                Id = existing.Id.Value,
                OtherUserId = request.RecipientId,
                OtherUserName = request.RecipientName,
                LastMessagePreview = existing.Messages.LastOrDefault()?.Content ?? "",
                UpdatedAt = existing.UpdatedAt
            });

        var conversationResult = Conversation.Create(
            request.SenderId,
            request.SenderName,
            request.RecipientId,
            request.RecipientName);

        if (conversationResult.IsFailure)
            return Result.Failure<ConversationDto>(conversationResult.Error);

        var conversation = conversationResult.Value;

        var messageResult = Message.Create(
            conversation.Id,
            request.SenderId,
            request.SenderName,
            request.InitialMessage);

        if (messageResult.IsFailure)
            return Result.Failure<ConversationDto>(messageResult.Error);

        var addResult = conversation.AddMessage(messageResult.Value);
        if (addResult.IsFailure)
            return Result.Failure<ConversationDto>(addResult.Error);

        await conversationRepository.AddAsync(conversation, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new ConversationDto
        {
            Id = conversation.Id.Value,
            OtherUserId = request.RecipientId,
            OtherUserName = request.RecipientName,
            LastMessagePreview = request.InitialMessage,
            UpdatedAt = conversation.UpdatedAt
        });
    }
}