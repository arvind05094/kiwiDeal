using kiwiDeal.Payments.Domain.Entities;
using kiwiDeal.Payments.Domain.Repositories;
using kiwiDeal.SharedKernel.Contracts;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Payments.Application.Queries.GetPaymentStatusByAuction;

public sealed class GetPaymentStatusByAuctionQueryHandler(
    IPaymentRepository paymentRepository)
    : IRequestHandler<GetPaymentStatusByAuctionQuery, Result<string>>
{
    public async Task<Result<string>> Handle(
        GetPaymentStatusByAuctionQuery request,
        CancellationToken cancellationToken)
    {
        var payment = await paymentRepository.GetByAuctionIdAsync(
            request.AuctionId, cancellationToken);

        return Result.Success(payment?.Status.ToString() ?? PaymentStatus.Pending.ToString());
    }
}
