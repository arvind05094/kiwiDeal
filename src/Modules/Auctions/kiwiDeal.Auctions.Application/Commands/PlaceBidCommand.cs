using FluentValidation;
using kiwiDeal.Auctions.Domain.Entities;
using kiwiDeal.Auctions.Domain.Errors;
using kiwiDeal.Auctions.Domain.Repositories;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Auctions.Application.Commands;

public sealed record PlaceBidCommand(
    Guid AuctionId,
    Guid BidderId,
    decimal Amount) : IRequest<Result>;

public interface IAuctionHubContext
{
    Task SendBidPlaced(string auctionId, Guid bidId, Guid bidderId, decimal amount, DateTimeOffset newEndTime, CancellationToken cancellationToken = default);
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

        var result = auction.PlaceBid(command.BidderId, command.Amount);

        if (result.IsFailure)
            return result;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send real-time update after transaction commits
        var latestBid = auction.Bids.Last();
        await _hubContext.SendBidPlaced(
            command.AuctionId.ToString(),
            latestBid.Id.Value,
            command.BidderId,
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
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}
