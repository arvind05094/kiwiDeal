using kiwiDeal.Auctions.Application.DTOs;
using kiwiDeal.Auctions.Domain.Entities;
using kiwiDeal.Auctions.Domain.Errors;
using kiwiDeal.Auctions.Domain.Repositories;
using kiwiDeal.SharedKernel.Interfaces;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Auctions.Application.Queries;

public sealed record GetAuctionQuery(Guid AuctionId) : IRequest<Result<AuctionDto>>, IPublicRequest;

public sealed class GetAuctionQueryHandler : IRequestHandler<GetAuctionQuery, Result<AuctionDto>>
{
    private readonly IAuctionRepository _auctionRepository;

    public GetAuctionQueryHandler(IAuctionRepository auctionRepository)
    {
        _auctionRepository = auctionRepository;
    }

    public async Task<Result<AuctionDto>> Handle(GetAuctionQuery query, CancellationToken cancellationToken)
    {
        var auction = await _auctionRepository.GetByIdAsync(
            AuctionId.From(query.AuctionId), cancellationToken);

        if (auction is null)
            return Result.Failure<AuctionDto>(AuctionErrors.NotFound(query.AuctionId));

        return Result.Success(new AuctionDto(
            auction.Id.Value,
            auction.ListingId,
            auction.ListingTitle,
            auction.SellerId,
            auction.StartingPrice,
            auction.CurrentHighestBid,
            auction.CurrentHighestBidderId,
            auction.StartTime,
            auction.EndTime,
            auction.ClosedAt,
            auction.Status.ToString(),
            auction.Bids
                .OrderBy(b => b.CreatedAt)
                .Select(b => new AuctionBidDto(
                    b.Id.Value,
                    b.BidderId,
                    b.BidderName,
                    b.Amount,
                    b.CreatedAt))
                .ToList(),null));
    }
}