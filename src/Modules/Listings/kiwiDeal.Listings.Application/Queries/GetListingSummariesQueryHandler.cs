using kiwiDeal.Listings.Domain.Repositories;
using kiwiDeal.SharedKernel.Contracts;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Listings.Application.Queries;

public sealed class GetListingSummariesQueryHandler(
    IListingRepository listingRepository)
    : IRequestHandler<GetListingSummariesQuery, Result<List<ListingSummaryDto>>>
{
    public async Task<Result<List<ListingSummaryDto>>> Handle(
        GetListingSummariesQuery request, CancellationToken cancellationToken)
    {
        if (request.ListingIds.Count == 0)
            return Result.Success(new List<ListingSummaryDto>());

        var listings = await listingRepository.GetByIdsAsync(request.ListingIds, cancellationToken);

        var dtos = listings.Select(l => new ListingSummaryDto(
            l.Id.Value,
            l.Title,
            l.Images.OrderBy(i => i.DisplayOrder).FirstOrDefault()?.Url))
            .ToList();

        return Result.Success(dtos);
    }
}