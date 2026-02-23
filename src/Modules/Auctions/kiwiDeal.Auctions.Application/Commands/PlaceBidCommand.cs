using FluentValidation;
using kiwiDeal.Auctions.Domain.Entities;
using kiwiDeal.Auctions.Domain.Errors;
using kiwiDeal.Auctions.Domain.Repositories;
using kiwiDeal.SharedKernel.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace kiwiDeal.Auctions.Application.Commands;

public sealed record PlaceBidCommand(
    Guid AuctionId,
    Guid BidderId,
    string BidderName,
    decimal Amount) : IRequest<Result>;

public interface IAuctionHubContext
{
    Task SendBidPlaced(string auctionId, Guid bidId, Guid bidderId, string bidderName, decimal amount, DateTimeOffset newEndTime, CancellationToken cancellationToken = default);
    Task SendAuctionClosed(string auctionId, Guid? winnerId, decimal? finalAmount, CancellationToken cancellationToken = default);
}
public sealed class PlaceBidCommandHandler : IRequestHandler<PlaceBidCommand, Result>
{
    private readonly IAuctionRepository _auctionRepository;
    private readonly IAuctionsUnitOfWork _unitOfWork;
    private readonly IAuctionHubContext _hubContext;

    public PlaceBidCommandHandler(
        IAuctionRepository auctionRepository,
        IAuctionsUnitOfWork unitOfWork,
        IAuctionHubContext hubContext)
    {
        _auctionRepository = auctionRepository;
        _unitOfWork = unitOfWork;
        _hubContext = hubContext;
    }

    public async Task<Result> Handle(PlaceBidCommand command, CancellationToken cancellationToken)
    {
        var auction = await _auctionRepository.GetByIdAsync(
            AuctionId.From(command.AuctionId), cancellationToken);

        if (auction is null)
            return Result.Failure(AuctionErrors.NotFound(command.AuctionId));

        var result = auction.PlaceBid(command.BidderId, command.BidderName, command.Amount);
        if (result.IsFailure)
            return result;

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Failure(AuctionErrors.BidConflict());
        }

        var latestBid = auction.Bids.Last();
        await _hubContext.SendBidPlaced(
            command.AuctionId.ToString(),
            latestBid.Id.Value,
            command.BidderId,
            command.BidderName,
            command.Amount,
            auction.EndTime,
            cancellationToken);

        return Result.Success();
    }
}

public sealed class PlaceBidCommandValidator : AbstractValidator<PlaceBidCommand>
{
    public PlaceBidCommandValidator()
    {
        RuleFor(x => x.AuctionId).NotEmpty();
        RuleFor(x => x.BidderId).NotEmpty();
        RuleFor(x => x.BidderName).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}
