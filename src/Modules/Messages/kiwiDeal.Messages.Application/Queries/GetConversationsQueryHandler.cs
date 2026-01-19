using kiwiDeal.Messages.Application.DTOs;
using kiwiDeal.Messages.Domain.Repositories;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Messages.Application.Queries;

public class GetConversationsQueryHandler(
    IConversationRepository conversationRepository)
    : IRequestHandler<GetConversationsQuery, Result<List<ConversationDto>>>
{
    public async Task<Result<List<ConversationDto>>> Handle(
        GetConversationsQuery request,
        CancellationToken cancellationToken)
    {
        var conversations = await conversationRepository.GetByUserIdAsync(
            request.UserId, cancellationToken);

        var dtos = conversations.Select(c => new ConversationDto
        {
            Id = c.Id.Value,
            ListingId = c.ListingId,
            OtherUserId = c.SenderId == request.UserId ? c.RecipientId : c.SenderId,
            OtherUserName = string.Empty,
            ListingTitle = string.Empty,
            LastMessagePreview = string.Empty,
            UpdatedAt = c.UpdatedAt
        }).ToList();

        return Result.Success(dtos);
    }
}
