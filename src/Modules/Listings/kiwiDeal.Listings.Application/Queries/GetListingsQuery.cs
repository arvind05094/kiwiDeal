using kiwiDeal.Listings.Application.DTOs;
using kiwiDeal.Listings.Domain.Repositories;
using kiwiDeal.SharedKernel.Pagination;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Listings.Application.Queries;

public sealed record GetListingsQuery(
    int PageNumber,
    int PageSize,
    string? SearchTerm) : IRequest<Result<PagedResult<ListingDto>>>;

public sealed class GetListingsQueryHandler : IRequestHandler<GetListingsQuery, Result<PagedResult<ListingDto>>>
{
    private readonly IListingRepository _listingRepository;

    public GetListingsQueryHandler(IListingRepository listingRepository)
    {
        _listingRepository = listingRepository;
    }

    public async Task<Result<PagedResult<ListingDto>>> Handle(GetListingsQuery query, CancellationToken cancellationToken)
    {
        var pagination = new PaginationParams(query.PageNumber, query.PageSize);

        var pagedListings = await _listingRepository.GetAllAsync(pagination, query.SearchTerm, cancellationToken);

        var dtos = pagedListings.Items.Select(listing => new ListingDto(
            listing.Id.Value,
            listing.SellerId.Value,
            listing.Title,
            listing.Description,
            listing.StartingPrice,
            listing.Status.ToString(),
            listing.CreatedAt,
            listing.UpdatedAt,
            listing.Images.Select(i => new ListingImageDto(i.Url, i.DisplayOrder)).ToList())).ToList();

        var result = PagedResult<ListingDto>.Create(dtos, pagedListings.TotalCount, pagination);

        return Result.Success(result);
    }
}
