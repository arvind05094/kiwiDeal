using FluentValidation;
using kiwiDeal.Auctions.Domain.Entities;
using kiwiDeal.Auctions.Domain.Errors;
using kiwiDeal.Auctions.Domain.Repositories;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Auctions.Application.Commands;

public sealed record CloseAuctionCommand(Guid AuctionId, Guid SellerId) : IRequest<Result>;

public sealed class CloseAuctionCommandHandler : IRequestHandler<CloseAuctionCommand, Result>
{
    private readonly IAuctionRepository _auctionRepository;
    private readonly IAuctionsUnitOfWork _unitOfWork;

    public CloseAuctionCommandHandler(IAuctionRepository auctionRepository, IAuctionsUnitOfWork unitOfWork)
    {
        _auctionRepository = auctionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CloseAuctionCommand command, CancellationToken cancellationToken)
    {
        var auction = await _auctionRepository.GetByIdAsync(
            AuctionId.From(command.AuctionId), cancellationToken);

        if (auction is null)
            return Result.Failure(AuctionErrors.NotFound(command.AuctionId));

        if (auction.SellerId != command.SellerId)
            return Result.Failure(AuctionErrors.Forbidden());

        var result = auction.Close();

        if (result.IsFailure)
            return result;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

public sealed class CloseAuctionCommandValidator : AbstractValidator<CloseAuctionCommand>
{
    public CloseAuctionCommandValidator()
    {
        RuleFor(x => x.AuctionId).NotEmpty();
        RuleFor(x => x.SellerId).NotEmpty();
    }
}
