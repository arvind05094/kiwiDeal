using FluentValidation;
using kiwiDeal.Listings.Domain.Errors;
using kiwiDeal.Listings.Domain.Repositories;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Listings.Application.Commands;

public sealed record RemoveFromAuctionWatchlistCommand(
    Guid UserId,
    Guid AuctionId) : IRequest<Result>;

public sealed class RemoveFromAuctionWatchlistCommandHandler(
    IListingRepository listingRepository,
    IListingsUnitOfWork unitOfWork) : IRequestHandler<RemoveFromAuctionWatchlistCommand, Result>
{
    public async Task<Result> Handle(RemoveFromAuctionWatchlistCommand command, CancellationToken cancellationToken)
    {
        var entry = await listingRepository.GetAuctionWatchlistEntryAsync(
            command.UserId, command.AuctionId, cancellationToken);

        if (entry is null)
            return Result.Failure(ListingErrors.AuctionWatchlistEntryNotFound());

        listingRepository.RemoveAuctionWatchlistEntry(entry);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

public sealed class RemoveFromAuctionWatchlistCommandValidator : AbstractValidator<RemoveFromAuctionWatchlistCommand>
{
    public RemoveFromAuctionWatchlistCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.AuctionId).NotEmpty();
    }
}
