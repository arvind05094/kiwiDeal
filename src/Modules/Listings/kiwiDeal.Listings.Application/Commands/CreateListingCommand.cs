using FluentValidation;
using kiwiDeal.Listings.Domain.Entities;
using kiwiDeal.Listings.Domain.Repositories;
using kiwiDeal.SharedKernel.Interfaces;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Listings.Application.Commands;

public sealed record CreateListingCommand(
    Guid SellerId,
    string Title,
    string Description,
    decimal StartingPrice) : IRequest<Result<Guid>>;

public sealed class CreateListingCommandHandler : IRequestHandler<CreateListingCommand, Result<Guid>>
{
    private readonly IListingRepository _listingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateListingCommandHandler(IListingRepository listingRepository, IUnitOfWork unitOfWork)
    {
        _listingRepository = listingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateListingCommand command, CancellationToken cancellationToken)
    {
        var sellerId = SellerId.From(command.SellerId);

        var result = Listing.Create(
            sellerId,
            command.Title,
            command.Description,
            command.StartingPrice);

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

        RuleFor(x => x.StartingPrice)
            .GreaterThanOrEqualTo(0);
    }
}
