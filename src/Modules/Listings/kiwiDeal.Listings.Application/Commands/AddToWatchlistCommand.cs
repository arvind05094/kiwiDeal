using FluentValidation;
using kiwiDeal.Listings.Domain.Entities;
using kiwiDeal.Listings.Domain.Errors;
using kiwiDeal.Listings.Domain.Repositories;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Listings.Application.Commands;

public sealed record AddToWatchlistCommand(
    Guid UserId,
    Guid ListingId) : IRequest<Result>;

public sealed class AddToWatchlistCommandHandler(
    IListingRepository listingRepository,
    IListingsUnitOfWork unitOfWork) : IRequestHandler<AddToWatchlistCommand, Result>
{
    public async Task<Result> Handle(AddToWatchlistCommand command, CancellationToken cancellationToken)
    {
        var listingId = ListingId.From(command.ListingId);

        var listing = await listingRepository.GetByIdAsync(listingId, cancellationToken);
        if (listing is null)
            return Result.Failure(ListingErrors.NotFound(command.ListingId));

        if (listing.SellerId.Value == command.UserId)
            return Result.Failure(ListingErrors.CannotWatchOwnListing());

        if (listing.Status != ListingStatus.Active)
            return Result.Failure(ListingErrors.NotActive());

        var existing = await listingRepository.GetWatchlistEntryAsync(command.UserId, listingId, cancellationToken);
        if (existing is not null)
            return Result.Failure(ListingErrors.AlreadyWatching());

        var entry = ListingWatchlist.Create(command.UserId, listingId);
        await listingRepository.AddWatchlistEntryAsync(entry, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

public sealed class AddToWatchlistCommandValidator : AbstractValidator<AddToWatchlistCommand>
{
    public AddToWatchlistCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ListingId).NotEmpty();
    }
}
