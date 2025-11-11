using FluentValidation;
using kiwiDeal.Listings.Domain.Errors;
using kiwiDeal.Listings.Domain.Repositories;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Listings.Application.Commands;

public sealed record UpdateListingCommand(
    Guid ListingId,
    Guid SellerId,
    string Title,
    string Description,
    decimal StartingPrice) : IRequest<Result>;

public sealed class UpdateListingCommandHandler : IRequestHandler<UpdateListingCommand, Result>
{
    private readonly IListingRepository _listingRepository;
    private readonly IListingsUnitOfWork _unitOfWork;
    public UpdateListingCommandHandler(IListingRepository listingRepository, IListingsUnitOfWork unitOfWork)
    {
        _listingRepository = listingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateListingCommand command, CancellationToken cancellationToken)
    {
        var listingId = kiwiDeal.Listings.Domain.Entities.ListingId.From(command.ListingId);
        var listing = await _listingRepository.GetByIdAsync(listingId, cancellationToken);

        if (listing is null)
            return Result.Failure(ListingErrors.NotFound(command.ListingId));

        if (listing.SellerId.Value != command.SellerId)
            return Result.Failure(ListingErrors.Forbidden());

        var result = listing.Update(command.Title, command.Description, command.StartingPrice);

        if (result.IsFailure)
            return result;

        _listingRepository.Update(listing);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

public sealed class UpdateListingCommandValidator : AbstractValidator<UpdateListingCommand>
{
    public UpdateListingCommandValidator()
    {
        RuleFor(x => x.ListingId)
            .NotEmpty();

        RuleFor(x => x.SellerId)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(5000);

        RuleFor(x => x.StartingPrice)
            .GreaterThanOrEqualTo(0);
    }
}
