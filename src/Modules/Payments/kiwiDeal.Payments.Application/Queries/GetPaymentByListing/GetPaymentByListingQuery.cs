using kiwiDeal.Payments.Application.DTOs;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Payments.Application.Queries.GetPaymentByListing;

public sealed record GetPaymentByListingQuery(Guid ListingId) : IRequest<Result<PaymentDto>>;
