using FluentValidation;
using kiwiDeal.Listings.Domain.Entities;
using kiwiDeal.Listings.Domain.Enums;
using kiwiDeal.Listings.Domain.Repositories;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Listings.Application.Commands;

public sealed record CreateListingCommand(
    Guid SellerId,
    string SellerName,
    string Title,
    string Description,
    ListingType ListingType,
    decimal? BuyNowPrice,
    ListingCategory Category,
    ListingRegion Region) : IRequest<Result<Guid>>;
public sealed class CreateListingCommandHandler : IRequestHandler<CreateListingCommand, Result<Guid>>
{
    private readonly IListingRepository _listingRepository;
    private readonly IListingsUnitOfWork _unitOfWork;
    public CreateListingCommandHandler(IListingRepository listingRepository, IListingsUnitOfWork unitOfWork)
    {
        _listingRepository = listingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateListingCommand command, CancellationToken cancellationToken)
    {
        var sellerId = SellerId.From(command.SellerId);

        var result = Listing.Create(
            sellerId,
            command.SellerName,
            command.Title,
            command.Description,
            command.ListingType,
            command.BuyNowPrice,
            command.Category,
            command.Region);

        if (result.IsFailure)
            return Result.Failure<Guid>(result.Error);

        await _listingRepository.AddAsync(result.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(result.Value.Id.Value);
    }
}

public sealed class CreateListingCommandValidator : AbstractValidator<CreateListingCommand>
{
    public CreateListingCommandValidator()
    {
        RuleFor(x => x.SellerId)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(5000);

        RuleFor(x => x.BuyNowPrice)
            .GreaterThan(0)
            .When(x => x.ListingType == ListingType.FixedPrice);
    }
}
