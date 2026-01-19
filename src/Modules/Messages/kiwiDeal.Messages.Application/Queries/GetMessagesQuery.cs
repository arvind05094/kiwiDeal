using kiwiDeal.Messages.Application.DTOs;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Messages.Application.Queries;

public record GetMessagesQuery(Guid ConversationId, Guid UserId) : IRequest<Result<List<MessageDto>>>;
