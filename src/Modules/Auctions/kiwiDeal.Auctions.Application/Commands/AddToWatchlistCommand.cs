using FluentValidation;
using kiwiDeal.Auctions.Domain.Entities;
using kiwiDeal.Auctions.Domain.Errors;
using kiwiDeal.Auctions.Domain.Repositories;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Auctions.Application.Commands;

public sealed record AddToWatchlistCommand(
    Guid UserId,
    Guid AuctionId,
    Guid AuctionSellerId,
    string AuctionStatus) : IRequest<Result>;

public sealed class AddToWatchlistCommandHandler(
    IAuctionWatchlistRepository watchlistRepository,
    IAuctionsUnitOfWork unitOfWork) : IRequestHandler<AddToWatchlistCommand, Result>
{
    public async Task<Result> Handle(AddToWatchlistCommand command, CancellationToken cancellationToken)
    {
        if (command.AuctionSellerId == command.UserId)
            return Result.Failure(AuctionWatchlistErrors.CannotWatchOwnAuction());

        if (command.AuctionStatus != "Scheduled" && command.AuctionStatus != "Active")
            return Result.Failure(AuctionWatchlistErrors.AuctionNotWatchable());

        var existing = await watchlistRepository.GetEntryAsync(
            command.UserId, command.AuctionId, cancellationToken);

        if (existing is not null)
            return Result.Failure(AuctionWatchlistErrors.AlreadyWatching());

        var entry = AuctionWatchlist.Create(command.UserId, command.AuctionId);
        await watchlistRepository.AddAsync(entry, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

public sealed class AddToWatchlistCommandValidator : AbstractValidator<AddToWatchlistCommand>
{
    public AddToWatchlistCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.AuctionId).NotEmpty();
        RuleFor(x => x.AuctionSellerId).NotEmpty();
        RuleFor(x => x.AuctionStatus).NotEmpty();
    }
}
