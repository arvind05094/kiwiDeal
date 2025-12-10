using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Payments.Application.Commands.CreateCheckoutSession;

public sealed record CreateCheckoutSessionCommand(Guid AuctionId) : IRequest<Result<string>>;
