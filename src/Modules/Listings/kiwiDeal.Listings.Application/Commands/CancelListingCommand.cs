using FluentValidation;
using kiwiDeal.Listings.Domain.Errors;
using kiwiDeal.Listings.Domain.Repositories;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Listings.Application.Commands;

public sealed record CancelListingCommand(
    Guid ListingId,
    Guid SellerId) : IRequest<Result>;

public sealed class CancelListingCommandHandler : IRequestHandler<CancelListingCommand, Result>
{
    private readonly IListingRepository _listingRepository;
    private readonly IListingsUnitOfWork _unitOfWork;

    public CancelListingCommandHandler(IListingRepository listingRepository, IListingsUnitOfWork unitOfWork)
    {
        _listingRepository = listingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CancelListingCommand command, CancellationToken cancellationToken)
    {
        var listingId = kiwiDeal.Listings.Domain.Entities.ListingId.From(command.ListingId);
        var listing = await _listingRepository.GetByIdAsync(listingId, cancellationToken);

        if (listing is null)
            return Result.Failure(ListingErrors.NotFound(command.ListingId));

        if (listing.SellerId.Value != command.SellerId)
            return Result.Failure(ListingErrors.Forbidden());

        var result = listing.Close();
        if (result.IsFailure)
            return result;

        _listingRepository.Update(listing);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

public sealed class CancelListingCommandValidator : AbstractValidator<CancelListingCommand>
{
    public CancelListingCommandValidator()
    {
        RuleFor(x => x.ListingId).NotEmpty();
        RuleFor(x => x.SellerId).NotEmpty();
    }
}
