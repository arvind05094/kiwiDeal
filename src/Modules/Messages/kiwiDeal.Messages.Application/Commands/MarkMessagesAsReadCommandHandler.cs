using kiwiDeal.Messages.Domain.Entities;
using kiwiDeal.Messages.Domain.Errors;
using kiwiDeal.Messages.Domain.Repositories;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Messages.Application.Commands;

public class MarkMessagesAsReadCommandHandler(
    IConversationRepository conversationRepository) : IRequestHandler<MarkMessagesAsReadCommand, Result>
{
    public async Task<Result> Handle(
        MarkMessagesAsReadCommand request,
        CancellationToken cancellationToken)
    {
        var conversation = await conversationRepository.GetByIdAsync(
            ConversationId.From(request.ConversationId), cancellationToken);

        if (conversation is null)
            return Result.Failure(MessageErrors.ConversationNotFound(request.ConversationId));

        if (!conversation.IsParticipant(request.UserId))
            return Result.Failure(MessageErrors.NotParticipant);

        await conversationRepository.MarkMessagesAsReadAsync(
            conversation.Id, request.UserId, cancellationToken);

        return Result.Success();
    }
}
