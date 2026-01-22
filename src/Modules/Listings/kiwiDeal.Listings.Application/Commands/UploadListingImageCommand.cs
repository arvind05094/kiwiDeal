using FluentValidation;
using kiwiDeal.Listings.Domain.Entities;
using kiwiDeal.Listings.Domain.Errors;
using kiwiDeal.Listings.Domain.Repositories;
using kiwiDeal.Listings.Domain.ValueObjects;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Listings.Application.Commands;

public sealed record UploadListingImageCommand(
    Guid ListingId,
    Guid SellerId,
    Stream ImageStream,
    string FileName,
    string ContentType) : IRequest<Result<string>>;

public sealed class UploadListingImageCommandHandler(
    IListingRepository listingRepository,
    IImageService imageService,
    IListingsUnitOfWork unitOfWork) : IRequestHandler<UploadListingImageCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UploadListingImageCommand command, CancellationToken cancellationToken)
    {
        var listingId = ListingId.From(command.ListingId);
        var listing = await listingRepository.GetByIdAsync(listingId, cancellationToken);

        if (listing is null)
            return Result.Failure<string>(ListingErrors.NotFound(command.ListingId));

        if (listing.SellerId.Value != command.SellerId)
            return Result.Failure<string>(ListingErrors.NotOwner());

        var url = await imageService.UploadImageAsync(
            command.ListingId,
            command.ImageStream,
            command.FileName,
            command.ContentType,
            cancellationToken);

        var image = ListingImage.Create(url, listing.Images.Count);
        var result = listing.AddImage(image);

        if (result.IsFailure)
            return Result.Failure<string>(result.Error);

        listingRepository.Update(listing);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(url);
    }
}

public sealed class UploadListingImageCommandValidator : AbstractValidator<UploadListingImageCommand>
{
    public UploadListingImageCommandValidator()
    {
        RuleFor(x => x.ListingId).NotEmpty();
        RuleFor(x => x.SellerId).NotEmpty();
        RuleFor(x => x.FileName).NotEmpty();
        RuleFor(x => x.ContentType).NotEmpty();
    }
}
