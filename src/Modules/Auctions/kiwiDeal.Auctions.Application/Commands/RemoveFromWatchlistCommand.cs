using FluentValidation;
using kiwiDeal.Auctions.Domain.Errors;
using kiwiDeal.Auctions.Domain.Repositories;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Auctions.Application.Commands;

public sealed record RemoveFromWatchlistCommand(
    Guid UserId,
    Guid AuctionId) : IRequest<Result>;

public sealed class RemoveFromWatchlistCommandHandler(
    IAuctionWatchlistRepository watchlistRepository,
    IAuctionsUnitOfWork unitOfWork) : IRequestHandler<RemoveFromWatchlistCommand, Result>
{
    public async Task<Result> Handle(RemoveFromWatchlistCommand command, CancellationToken cancellationToken)
    {
        var entry = await watchlistRepository.GetEntryAsync(
            command.UserId, command.AuctionId, cancellationToken);

        if (entry is null)
            return Result.Failure(AuctionWatchlistErrors.NotFound());

        watchlistRepository.Remove(entry);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

public sealed class RemoveFromWatchlistCommandValidator : AbstractValidator<RemoveFromWatchlistCommand>
{
    public RemoveFromWatchlistCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.AuctionId).NotEmpty();
    }
}
