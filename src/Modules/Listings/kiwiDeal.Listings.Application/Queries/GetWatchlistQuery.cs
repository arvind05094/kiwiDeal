using kiwiDeal.Listings.Application.DTOs;
using kiwiDeal.Listings.Domain.Repositories;
using kiwiDeal.SharedKernel.Pagination;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Listings.Application.Queries;

public sealed record GetWatchlistQuery(
    Guid UserId,
    int PageNumber,
    int PageSize) : IRequest<Result<PagedResult<WatchlistItemDto>>>;

public sealed class GetWatchlistQueryHandler(
    IListingRepository listingRepository) : IRequestHandler<GetWatchlistQuery, Result<PagedResult<WatchlistItemDto>>>
{
    public async Task<Result<PagedResult<WatchlistItemDto>>> Handle(GetWatchlistQuery query, CancellationToken cancellationToken)
    {
        var pagination = new PaginationParams(query.PageNumber, query.PageSize);
        var paged = await listingRepository.GetWatchlistByUserIdAsync(query.UserId, pagination, cancellationToken);

        var dtos = paged.Items.Select(w => new WatchlistItemDto(
            w.ListingId.Value,
            w.Listing.Title,
            w.Listing.Status.ToString(),
            w.Listing.ListingType.ToString(),
            w.Listing.BuyNowPrice,
            w.Listing.Images.FirstOrDefault()?.Url,
            w.CreatedAt)).ToList();

        return Result.Success(PagedResult<WatchlistItemDto>.Create(dtos, paged.TotalCount, pagination));
    }
}
