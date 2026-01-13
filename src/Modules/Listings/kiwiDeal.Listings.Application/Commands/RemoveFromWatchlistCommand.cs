using FluentValidation;
using kiwiDeal.Listings.Domain.Entities;
using kiwiDeal.Listings.Domain.Errors;
using kiwiDeal.Listings.Domain.Repositories;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Listings.Application.Commands;

public sealed record RemoveFromWatchlistCommand(
    Guid UserId,
    Guid ListingId) : IRequest<Result>;

public sealed class RemoveFromWatchlistCommandHandler(
    IListingRepository listingRepository,
    IListingsUnitOfWork unitOfWork) : IRequestHandler<RemoveFromWatchlistCommand, Result>
{
    public async Task<Result> Handle(RemoveFromWatchlistCommand command, CancellationToken cancellationToken)
    {
        var listingId = ListingId.From(command.ListingId);

        var entry = await listingRepository.GetWatchlistEntryAsync(command.UserId, listingId, cancellationToken);
        if (entry is null)
            return Result.Failure(ListingErrors.WatchlistEntryNotFound());

        listingRepository.RemoveWatchlistEntry(entry);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

public sealed class RemoveFromWatchlistCommandValidator : AbstractValidator<RemoveFromWatchlistCommand>
{
    public RemoveFromWatchlistCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ListingId).NotEmpty();
    }
}
