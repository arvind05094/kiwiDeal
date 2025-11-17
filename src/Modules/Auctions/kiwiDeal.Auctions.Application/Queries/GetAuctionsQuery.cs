using kiwiDeal.Auctions.Application.DTOs;
using kiwiDeal.Auctions.Domain.Repositories;
using kiwiDeal.SharedKernel.Interfaces;
using kiwiDeal.SharedKernel.Pagination;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Auctions.Application.Queries;

public sealed record GetAuctionsQuery(int PageNumber, int PageSize) : IRequest<Result<PagedResult<AuctionDto>>>, IPublicRequest;

public sealed class GetAuctionsQueryHandler : IRequestHandler<GetAuctionsQuery, Result<PagedResult<AuctionDto>>>
{
    private readonly IAuctionRepository _auctionRepository;

    public GetAuctionsQueryHandler(IAuctionRepository auctionRepository)
    {
        _auctionRepository = auctionRepository;
    }

    public async Task<Result<PagedResult<AuctionDto>>> Handle(GetAuctionsQuery query, CancellationToken cancellationToken)
    {
        var pagination = new PaginationParams(query.PageNumber, query.PageSize);
        var pagedAuctions = await _auctionRepository.GetPagedAsync(query.PageNumber, query.PageSize, cancellationToken);

        var dtos = pagedAuctions.Items.Select(a => new AuctionDto(
            a.Id.Value,
            a.ListingId,
            a.SellerId,
            a.StartingPrice,
            a.CurrentHighestBid,
            a.CurrentHighestBidderId,
            a.StartTime,
            a.EndTime,
            a.Status.ToString(),
            a.Bids.Select(b => new AuctionBidDto(
                b.Id.Value,
                b.BidderId,
                b.Amount,
                b.CreatedAt)).ToList())).ToList();

        return Result.Success(PagedResult<AuctionDto>.Create(dtos, pagedAuctions.TotalCount, pagination));
    }
}
