using kiwiDeal.Auctions.Application.DTOs;
using kiwiDeal.Auctions.Domain.Repositories;
using kiwiDeal.SharedKernel.Pagination;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Auctions.Application.Queries;

public sealed record GetWatchlistQuery(
    Guid UserId,
    int PageNumber,
    int PageSize) : IRequest<Result<PagedResult<AuctionWatchlistItemDto>>>;

public sealed class GetWatchlistQueryHandler(
    IAuctionWatchlistRepository watchlistRepository)
    : IRequestHandler<GetWatchlistQuery, Result<PagedResult<AuctionWatchlistItemDto>>>
{
    public async Task<Result<PagedResult<AuctionWatchlistItemDto>>> Handle(
        GetWatchlistQuery query, CancellationToken cancellationToken)
    {
        var pagination = new PaginationParams(query.PageNumber, query.PageSize);
        var paged = await watchlistRepository.GetByUserIdAsync(query.UserId, pagination, cancellationToken);

        var dtos = paged.Items
            .Select(w => new AuctionWatchlistItemDto(w.AuctionId, w.CreatedAt))
            .ToList();

        return Result.Success(PagedResult<AuctionWatchlistItemDto>.Create(dtos, paged.TotalCount, pagination));
    }
}
