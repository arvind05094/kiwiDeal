using kiwiDeal.Auctions.Application.DTOs;
using kiwiDeal.Auctions.Domain.Repositories;
using kiwiDeal.SharedKernel.Contracts;
using kiwiDeal.SharedKernel.Pagination;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Auctions.Application.Queries;

public sealed record GetBiddingAuctionsQuery(
    Guid BidderId,
    string? Status,
    int PageNumber,
    int PageSize) : IRequest<Result<PagedResult<AuctionDto>>>;

public sealed class GetBiddingAuctionsQueryHandler(
    IAuctionRepository auctionRepository,
    IMediator mediator)
    : IRequestHandler<GetBiddingAuctionsQuery, Result<PagedResult<AuctionDto>>>
{
    public async Task<Result<PagedResult<AuctionDto>>> Handle(
        GetBiddingAuctionsQuery query, CancellationToken cancellationToken)
    {
        var paged = await auctionRepository.GetByBidderIdAsync(
            query.BidderId, query.Status, query.PageNumber, query.PageSize, cancellationToken);

        var listingIds = paged.Items.Select(a => a.ListingId).Distinct().ToList();
        var summariesResult = await mediator.Send(new GetListingSummariesQuery(listingIds), cancellationToken);
        var summaries = summariesResult.IsSuccess
            ? summariesResult.Value.ToDictionary(s => s.Id)
            : new Dictionary<Guid, ListingSummaryDto>();

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
            [],
            summaries.GetValueOrDefault(a.ListingId)?.ThumbnailUrl)).ToList();

        return Result.Success(PagedResult<AuctionDto>.Create(
            dtos, paged.TotalCount, new PaginationParams(query.PageNumber, query.PageSize)));
    }
}