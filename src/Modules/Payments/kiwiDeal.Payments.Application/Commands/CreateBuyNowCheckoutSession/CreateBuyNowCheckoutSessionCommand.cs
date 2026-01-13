using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Payments.Application.Commands.CreateBuyNowCheckoutSession;

public sealed record CreateBuyNowCheckoutSessionCommand(
    Guid ListingId,
    Guid BuyerId,
    Guid SellerId,
    decimal Amount) : IRequest<Result<string>>;