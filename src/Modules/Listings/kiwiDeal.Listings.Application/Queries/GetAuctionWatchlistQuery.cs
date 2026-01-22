using kiwiDeal.Listings.Application.DTOs;
using kiwiDeal.Listings.Domain.Repositories;
using kiwiDeal.SharedKernel.Pagination;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Listings.Application.Queries;

public sealed record GetAuctionWatchlistQuery(
    Guid UserId,
    int PageNumber,
    int PageSize) : IRequest<Result<PagedResult<AuctionWatchlistItemDto>>>;

public sealed class GetAuctionWatchlistQueryHandler(
    IListingRepository listingRepository) : IRequestHandler<GetAuctionWatchlistQuery, Result<PagedResult<AuctionWatchlistItemDto>>>
{
    public async Task<Result<PagedResult<AuctionWatchlistItemDto>>> Handle(GetAuctionWatchlistQuery query, CancellationToken cancellationToken)
    {
        var pagination = new PaginationParams(query.PageNumber, query.PageSize);
        var paged = await listingRepository.GetAuctionWatchlistByUserIdAsync(query.UserId, pagination, cancellationToken);

        var dtos = paged.Items.Select(w => new AuctionWatchlistItemDto(
            w.AuctionId,
            w.CreatedAt)).ToList();

        return Result.Success(PagedResult<AuctionWatchlistItemDto>.Create(dtos, paged.TotalCount, pagination));
    }
}
