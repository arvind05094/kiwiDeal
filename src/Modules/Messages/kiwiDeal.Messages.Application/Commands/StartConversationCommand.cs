using kiwiDeal.Messages.Application.DTOs;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Messages.Application.Commands;

public record StartConversationCommand(
    Guid ListingId,
    string ListingTitle,
    Guid SenderId,
    string SenderName,
    Guid RecipientId,
    string RecipientName,
    string InitialMessage) : IRequest<Result<ConversationDto>>;
