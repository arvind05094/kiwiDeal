using kiwiDeal.Auctions.Application.DTOs;
using kiwiDeal.Auctions.Domain.Repositories;
using kiwiDeal.SharedKernel.Pagination;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Auctions.Application.Queries;

public sealed record GetBiddingAuctionsQuery(
    Guid BidderId,
    string? Status,
    int PageNumber,
    int PageSize) : IRequest<Result<PagedResult<AuctionDto>>>;

public sealed class GetBiddingAuctionsQueryHandler : IRequestHandler<GetBiddingAuctionsQuery, Result<PagedResult<AuctionDto>>>
{
    private readonly IAuctionRepository _auctionRepository;

    public GetBiddingAuctionsQueryHandler(IAuctionRepository auctionRepository)
    {
        _auctionRepository = auctionRepository;
    }

    public async Task<Result<PagedResult<AuctionDto>>> Handle(GetBiddingAuctionsQuery query, CancellationToken cancellationToken)
    {
        var paged = await _auctionRepository.GetByBidderIdAsync(
            query.BidderId, query.Status, query.PageNumber, query.PageSize, cancellationToken);

        var dtos = paged.Items.Select(a => new AuctionDto(
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

        return Result.Success(PagedResult<AuctionDto>.Create(dtos, paged.TotalCount, new PaginationParams(query.PageNumber, query.PageSize)));
    }
}