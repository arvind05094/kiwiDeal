using kiwiDeal.Listings.Domain.Entities;
using kiwiDeal.Listings.Domain.Repositories;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Listings.Application.Queries;

public sealed record IsListingWatchedQuery(
    Guid UserId,
    Guid ListingId) : IRequest<Result<bool>>;

public sealed class IsListingWatchedQueryHandler(
    IListingRepository listingRepository)
    : IRequestHandler<IsListingWatchedQuery, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        IsListingWatchedQuery query, CancellationToken cancellationToken)
    {
        var entry = await listingRepository.GetWatchlistEntryAsync(
            query.UserId,
            ListingId.From(query.ListingId),
            cancellationToken);

        return Result.Success(entry is not null);
    }
}
