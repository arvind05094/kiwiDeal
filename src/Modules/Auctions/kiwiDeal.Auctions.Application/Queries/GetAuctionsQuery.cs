using kiwiDeal.Auctions.Application.DTOs;
using kiwiDeal.Auctions.Domain.Repositories;
using kiwiDeal.SharedKernel.Interfaces;
using kiwiDeal.SharedKernel.Pagination;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Auctions.Application.Queries;

public sealed record GetAuctionsQuery(int PageNumber, int PageSize, bool EndingSoon = false) : IRequest<Result<PagedResult<AuctionDto>>>, IPublicRequest;

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
        var paged = await _auctionRepository.GetPagedAsync(query.PageNumber, query.PageSize, query.EndingSoon, cancellationToken);
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
            a.ClosedAt,
            a.Status.ToString(),
            [], null)).ToList();

        return Result.Success(PagedResult<AuctionDto>.Create(dtos, paged.TotalCount, pagination));
    }
}