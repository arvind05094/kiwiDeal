using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.SharedKernel.Contracts;

public sealed record GetPaymentStatusByAuctionQuery(Guid AuctionId)
    : IRequest<Result<string>>;
