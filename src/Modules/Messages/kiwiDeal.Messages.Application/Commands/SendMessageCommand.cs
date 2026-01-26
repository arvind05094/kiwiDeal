using kiwiDeal.Messages.Application.DTOs;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Messages.Application.Commands;

public record SendMessageCommand(
    Guid ConversationId,
    Guid SenderId,
    string SenderName,
    string Content) : IRequest<Result<MessageDto>>;
