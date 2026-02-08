using kiwiDeal.Payments.Application.DTOs;
using kiwiDeal.SharedKernel.Pagination;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Payments.Application.Queries.GetFixedPriceSales;

public sealed record GetFixedPriceSalesQuery(
    int PageNumber,
    int PageSize) : IRequest<Result<PagedResult<PaymentDto>>>;
