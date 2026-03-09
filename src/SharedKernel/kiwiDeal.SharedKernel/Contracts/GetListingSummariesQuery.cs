using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.SharedKernel.Contracts;

public sealed record ListingSummaryDto(Guid Id, string Title, string? ThumbnailUrl);

public sealed record GetListingSummariesQuery(List<Guid> ListingIds)
    : IRequest<Result<List<ListingSummaryDto>>>;