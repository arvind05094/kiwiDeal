using FluentValidation;
using kiwiDeal.Auctions.Domain.Entities;
using kiwiDeal.Auctions.Domain.Repositories;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Auctions.Application.Commands;

public sealed record CreateAuctionCommand(
    Guid ListingId,
    string ListingTitle,
    Guid SellerId,
    decimal StartingPrice,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime) : IRequest<Result<Guid>>;

public sealed class CreateAuctionCommandHandler : IRequestHandler<CreateAuctionCommand, Result<Guid>>
{
    private readonly IAuctionRepository _auctionRepository;
    private readonly IAuctionsUnitOfWork _unitOfWork;

    public CreateAuctionCommandHandler(IAuctionRepository auctionRepository, IAuctionsUnitOfWork unitOfWork)
    {
        _auctionRepository = auctionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateAuctionCommand command, CancellationToken cancellationToken)
    {
        var result = Auction.Create(
            command.ListingId,
            command.ListingTitle,
            command.SellerId,
            command.StartingPrice,
            command.StartTime.ToUniversalTime(),
            command.EndTime.ToUniversalTime());

        if (result.IsFailure)
            return Result.Failure<Guid>(result.Error);

        var auction = result.Value;

        if (command.StartTime <= DateTimeOffset.UtcNow)
            auction.Activate();

        _auctionRepository.Add(auction);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(auction.Id.Value);
    }
}

public sealed class CreateAuctionCommandValidator : AbstractValidator<CreateAuctionCommand>
{
    public CreateAuctionCommandValidator()
    {
        RuleFor(x => x.ListingId).NotEmpty();
        RuleFor(x => x.ListingTitle).NotEmpty().MaximumLength(200);
        RuleFor(x => x.SellerId).NotEmpty();
        RuleFor(x => x.StartingPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.StartTime).NotEmpty();
        RuleFor(x => x.EndTime)
            .NotEmpty()
            .GreaterThan(x => x.StartTime)
            .WithMessage("End time must be after start time.");
    }
}
