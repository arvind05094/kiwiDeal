using kiwiDeal.Payments.Application.DTOs;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Payments.Application.Queries.GetPayment;

public sealed record GetPaymentQuery(Guid AuctionId) : IRequest<Result<PaymentDto>>;
