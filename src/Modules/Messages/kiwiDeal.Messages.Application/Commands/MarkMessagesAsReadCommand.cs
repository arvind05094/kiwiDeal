using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Messages.Application.Commands;

public record MarkMessagesAsReadCommand(
    Guid ConversationId,
    Guid UserId) : IRequest<Result>;
