using kiwiDeal.Auctions.Application.DTOs;
using kiwiDeal.Auctions.Domain.Repositories;
using kiwiDeal.SharedKernel.Pagination;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Auctions.Application.Queries;

public sealed record GetWatchlistQuery(
    Guid UserId,
    int PageNumber,
    int PageSize) : IRequest<Result<PagedResult<AuctionDto>>>;

public sealed class GetWatchlistQueryHandler(
    IAuctionWatchlistRepository watchlistRepository,
    IAuctionRepository auctionRepository)
    : IRequestHandler<GetWatchlistQuery, Result<PagedResult<AuctionDto>>>
{
    public async Task<Result<PagedResult<AuctionDto>>> Handle(
        GetWatchlistQuery query, CancellationToken cancellationToken)
    {
        var pagination = new PaginationParams(query.PageNumber, query.PageSize);

        // Get watchlist entries (just IDs + metadata)
        var paged = await watchlistRepository.GetByUserIdAsync(query.UserId, pagination, cancellationToken);

        // Fetch full auction data for each watched auction
        var auctionIds = paged.Items.Select(w => w.AuctionId).ToList();
        var auctions = await auctionRepository.GetByIdsAsync(auctionIds, cancellationToken);

        var dtos = auctions.Select(a => new AuctionDto(
            a.Id.Value,
            a.ListingId,
            a.ListingTitle,
            a.SellerId,
            a.StartingPrice,
            a.CurrentHighestBid,
            a.CurrentHighestBidderId,
            a.StartTime,
            a.EndTime,
            a.Status.ToString(),
            [])).ToList();

        return Result.Success(PagedResult<AuctionDto>.Create(dtos, paged.TotalCount, pagination));
    }
}