using kiwiDeal.Messages.Application.DTOs;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Messages.Application.Queries;

public record GetConversationsQuery(Guid UserId) : IRequest<Result<List<ConversationDto>>>;
