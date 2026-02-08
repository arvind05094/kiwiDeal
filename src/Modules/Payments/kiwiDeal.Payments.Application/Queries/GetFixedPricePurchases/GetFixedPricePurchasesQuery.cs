using kiwiDeal.Payments.Application.DTOs;
using kiwiDeal.SharedKernel.Pagination;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Payments.Application.Queries.GetFixedPricePurchases;

public sealed record GetFixedPricePurchasesQuery(
    int PageNumber,
    int PageSize) : IRequest<Result<PagedResult<PaymentDto>>>;
