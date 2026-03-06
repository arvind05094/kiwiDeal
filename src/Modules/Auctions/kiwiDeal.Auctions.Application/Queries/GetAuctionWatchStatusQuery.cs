using kiwiDeal.Auctions.Domain.Repositories;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Auctions.Application.Queries;

public sealed record GetAuctionWatchStatusQuery(Guid UserId, Guid AuctionId) : IRequest<Result<bool>>;

public sealed class GetAuctionWatchStatusQueryHandler(IAuctionWatchlistRepository watchlistRepository)
    : IRequestHandler<GetAuctionWatchStatusQuery, Result<bool>>
{
    public async Task<Result<bool>> Handle(GetAuctionWatchStatusQuery query, CancellationToken cancellationToken)
    {
        var entry = await watchlistRepository.GetEntryAsync(query.UserId, query.AuctionId, cancellationToken);
        return Result.Success(entry is not null);
    }
}