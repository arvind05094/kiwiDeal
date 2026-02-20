using kiwiDeal.Payments.Application.DTOs;
using kiwiDeal.Payments.Domain.Repositories;
using kiwiDeal.SharedKernel.Interfaces;
using kiwiDeal.SharedKernel.Pagination;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Payments.Application.Queries.GetFixedPricePurchases;

public sealed class GetFixedPricePurchasesQueryHandler(
    IPaymentRepository paymentRepository,
    ICurrentUser currentUser) : IRequestHandler<GetFixedPricePurchasesQuery, Result<PagedResult<PaymentDto>>>
{
    public async Task<Result<PagedResult<PaymentDto>>> Handle(
        GetFixedPricePurchasesQuery request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await paymentRepository.GetFixedPricePurchasesByBuyerAsync(
            currentUser.Id!.Value, request.PageNumber, request.PageSize, cancellationToken);

        var dtos = items.Select(p => new PaymentDto(
            p.Id.Value, p.AuctionId, p.ListingId, p.BuyerId, p.SellerId,
            p.Amount, p.PaymentType, p.Status.ToString(),
            p.StripeSessionId, p.CreatedAt, p.PaidAt)).ToList();

        return Result.Success(PagedResult<PaymentDto>.Create(dtos, totalCount, new PaginationParams(request.PageNumber, request.PageSize)));
    }
}
