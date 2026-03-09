using kiwiDeal.Payments.Application.DTOs;
using kiwiDeal.Payments.Domain.Errors;
using kiwiDeal.Payments.Domain.Repositories;
using kiwiDeal.SharedKernel.Interfaces;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Payments.Application.Queries.GetPaymentByListing;

public sealed class GetPaymentByListingQueryHandler(
    IPaymentRepository paymentRepository,
    ICurrentUser currentUser) : IRequestHandler<GetPaymentByListingQuery, Result<PaymentDto>>
{
    public async Task<Result<PaymentDto>> Handle(
        GetPaymentByListingQuery request,
        CancellationToken cancellationToken)
    {
        var payment = await paymentRepository.GetByListingIdAsync(
            request.ListingId, cancellationToken);

        if (payment is null)
            return Result.Failure<PaymentDto>(PaymentErrors.NotFound);

        var callerId = currentUser.Id;
        if (callerId != payment.BuyerId && callerId != payment.SellerId)
            return Result.Failure<PaymentDto>(Error.Forbidden("You do not have permission to view this payment."));

        var dto = new PaymentDto(
            payment.Id.Value,
            payment.AuctionId,
            payment.ListingId,
            payment.BuyerId,
            payment.SellerId,
            payment.Amount,
            payment.PaymentType,
            payment.Status.ToString(),
            payment.StripeSessionId,
            payment.CreatedAt,
            payment.PaidAt,
            null,
            null);

        return Result.Success(dto);
    }
}
