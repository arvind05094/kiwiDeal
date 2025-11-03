using kiwiDeal.Listings.Application.DTOs;
using kiwiDeal.Listings.Domain.Errors;
using kiwiDeal.Listings.Domain.Repositories;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Listings.Application.Queries;

public sealed record GetListingQuery(Guid ListingId) : IRequest<Result<ListingDto>>;

public sealed class GetListingQueryHandler : IRequestHandler<GetListingQuery, Result<ListingDto>>
{
    private readonly IListingRepository _listingRepository;

    public GetListingQueryHandler(IListingRepository listingRepository)
    {
        _listingRepository = listingRepository;
    }

    public async Task<Result<ListingDto>> Handle(GetListingQuery query, CancellationToken cancellationToken)
    {
        var listingId = kiwiDeal.Listings.Domain.Entities.ListingId.From(query.ListingId);
        var listing = await _listingRepository.GetByIdAsync(listingId, cancellationToken);

        if (listing is null)
            return Result.Failure<ListingDto>(ListingErrors.NotFound(query.ListingId));

        var dto = new ListingDto(
            listing.Id.Value,
            listing.SellerId.Value,
            listing.Title,
            listing.Description,
            listing.StartingPrice,
            listing.Status.ToString(),
            listing.CreatedAt,
            listing.UpdatedAt,
            listing.Images.Select(i => new ListingImageDto(i.Url, i.DisplayOrder)).ToList());

        return Result.Success(dto);
    }
}
