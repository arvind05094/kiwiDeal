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

public sealed class PlaceBidCommandHandler : IRequestHandler<PlaceBidCommand, Result>
{
    private readonly IAuctionRepository _auctionRepository;
    private readonly IAuctionsUnitOfWork _unitOfWork;

    public PlaceBidCommandHandler(IAuctionRepository auctionRepository, IAuctionsUnitOfWork unitOfWork)
    {
        _auctionRepository = auctionRepository;
        _unitOfWork = unitOfWork;
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
