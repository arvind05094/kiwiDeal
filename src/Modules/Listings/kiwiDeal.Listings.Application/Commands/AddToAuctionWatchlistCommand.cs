using FluentValidation;
using kiwiDeal.Listings.Domain.Entities;
using kiwiDeal.Listings.Domain.Errors;
using kiwiDeal.Listings.Domain.Repositories;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Listings.Application.Commands;

public sealed record AddToAuctionWatchlistCommand(
    Guid UserId,
    Guid AuctionId,
    Guid AuctionSellerId,
    string AuctionStatus) : IRequest<Result>;

public sealed class AddToAuctionWatchlistCommandHandler(
    IListingRepository listingRepository,
    IListingsUnitOfWork unitOfWork) : IRequestHandler<AddToAuctionWatchlistCommand, Result>
{
    public async Task<Result> Handle(AddToAuctionWatchlistCommand command, CancellationToken cancellationToken)
    {
        if (command.AuctionSellerId == command.UserId)
            return Result.Failure(ListingErrors.CannotWatchOwnAuction());

        if (command.AuctionStatus != "Scheduled" && command.AuctionStatus != "Active")
            return Result.Failure(ListingErrors.AuctionNotWatchable());

        var existing = await listingRepository.GetAuctionWatchlistEntryAsync(
            command.UserId, command.AuctionId, cancellationToken);

        if (existing is not null)
            return Result.Failure(ListingErrors.AlreadyWatchingAuction());

        var entry = AuctionWatchlist.Create(command.UserId, command.AuctionId);
        await listingRepository.AddAuctionWatchlistEntryAsync(entry, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

public sealed class AddToAuctionWatchlistCommandValidator : AbstractValidator<AddToAuctionWatchlistCommand>
{
    public AddToAuctionWatchlistCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.AuctionId).NotEmpty();
        RuleFor(x => x.AuctionSellerId).NotEmpty();
        RuleFor(x => x.AuctionStatus).NotEmpty();
    }
}
