using kiwiDeal.Messages.Application.DTOs;
using kiwiDeal.Messages.Domain.Entities;
using kiwiDeal.Messages.Domain.Errors;
using kiwiDeal.Messages.Domain.Repositories;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Messages.Application.Queries;

public class GetMessagesQueryHandler(
    IConversationRepository conversationRepository)
    : IRequestHandler<GetMessagesQuery, Result<List<MessageDto>>>
{
    public async Task<Result<List<MessageDto>>> Handle(
        GetMessagesQuery request,
        CancellationToken cancellationToken)
    {
        var conversation = await conversationRepository.GetByIdAsync(
            ConversationId.From(request.ConversationId), cancellationToken);

        if (conversation is null)
            return Result.Failure<List<MessageDto>>(
                MessageErrors.ConversationNotFound(request.ConversationId));

        if (!conversation.IsParticipant(request.UserId))
            return Result.Failure<List<MessageDto>>(MessageErrors.NotParticipant);

        var dtos = conversation.Messages
            .OrderBy(m => m.CreatedAt)
            .Select(m => new MessageDto
            {
                Id = m.Id.Value,
                ConversationId = conversation.Id.Value,
                SenderId = m.SenderId,
                SenderName = string.Empty,
                Content = m.Content,
                CreatedAt = m.CreatedAt
            }).ToList();

        return Result.Success(dtos);
    }
}
