using kiwiDeal.Messages.Application.DTOs;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Messages.Application.Commands;

public record StartConversationCommand(
    Guid ListingId,
    Guid SenderId,
    Guid RecipientId,
    string InitialMessage) : IRequest<Result<ConversationDto>>;
