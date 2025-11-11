using FluentValidation;
using kiwiDeal.Listings.Domain.Errors;
using kiwiDeal.Listings.Domain.Repositories;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Listings.Application.Commands;

public sealed record DeleteListingCommand(
    Guid ListingId,
    Guid SellerId) : IRequest<Result>;

public sealed class DeleteListingCommandHandler : IRequestHandler<DeleteListingCommand, Result>
{
    private readonly IListingRepository _listingRepository;
    private readonly IListingsUnitOfWork _unitOfWork;
    public DeleteListingCommandHandler(IListingRepository listingRepository, IListingsUnitOfWork unitOfWork)
    {
        _listingRepository = listingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteListingCommand command, CancellationToken cancellationToken)
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

public sealed class DeleteListingCommandValidator : AbstractValidator<DeleteListingCommand>
{
    public DeleteListingCommandValidator()
    {
        RuleFor(x => x.ListingId)
            .NotEmpty();

        RuleFor(x => x.SellerId)
            .NotEmpty();
    }
}
