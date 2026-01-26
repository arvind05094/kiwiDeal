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

        var dtos = conversations.Select(c =>
        {
            var isSender = c.SenderId == request.UserId;
            var lastMessage = c.Messages.OrderByDescending(m => m.CreatedAt).FirstOrDefault();

            return new ConversationDto
            {
                Id = c.Id.Value,
                ListingId = c.ListingId,
                ListingTitle = c.ListingTitle,
                OtherUserId = isSender ? c.RecipientId : c.SenderId,
                OtherUserName = isSender ? c.RecipientName : c.SenderName,
                LastMessagePreview = lastMessage?.Content ?? string.Empty,
                UpdatedAt = c.UpdatedAt
            };
        }).ToList();

        return Result.Success(dtos);
    }
}
