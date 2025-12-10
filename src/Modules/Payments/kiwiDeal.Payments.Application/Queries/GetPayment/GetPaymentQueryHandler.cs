using kiwiDeal.Payments.Application.DTOs;
using kiwiDeal.Payments.Domain.Errors;
using kiwiDeal.Payments.Domain.Repositories;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Payments.Application.Queries.GetPayment;

public sealed class GetPaymentQueryHandler(
    IPaymentRepository paymentRepository) : IRequestHandler<GetPaymentQuery, Result<PaymentDto>>
{
    public async Task<Result<PaymentDto>> Handle(
        GetPaymentQuery request,
        CancellationToken cancellationToken)
    {
        var payment = await paymentRepository.GetByAuctionIdAsync(
            request.AuctionId, cancellationToken);

        if (payment is null)
            return Result.Failure<PaymentDto>(PaymentErrors.NotFound);

        var dto = new PaymentDto(
            payment.Id.Value,
            payment.AuctionId,
            payment.WinnerId,
            payment.SellerId,
            payment.Amount,
            payment.Status.ToString(),
            payment.StripeSessionId,
            payment.CreatedAt,
            payment.PaidAt);

        return Result.Success(dto);
    }
}
