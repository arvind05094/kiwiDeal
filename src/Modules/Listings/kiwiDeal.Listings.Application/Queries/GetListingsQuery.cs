using kiwiDeal.Listings.Application.DTOs;
using kiwiDeal.Listings.Domain.Repositories;
using kiwiDeal.SharedKernel.Interfaces;
using kiwiDeal.SharedKernel.Pagination;
using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Listings.Application.Queries;

public sealed record GetListingsQuery(
    int PageNumber,
    int PageSize,
    string? SearchTerm,
    string? Category,
    string? Region,
    string? SortBy,
    string? ListingType,
    Guid? SellerId) : IRequest<Result<PagedResult<ListingDto>>>, IPublicRequest;
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
        var pagedListings = await _listingRepository.GetAllAsync(pagination, query.SearchTerm, 
                query.Category, query.Region, query.SortBy, 
                query.ListingType, query.SellerId, cancellationToken);

        var dtos = pagedListings.Items.Select(listing => new ListingDto(
            listing.Id.Value,
            listing.SellerId.Value,
            listing.SellerName,
            listing.Title,
            listing.Description,
            listing.ListingType.ToString(),
            listing.BuyNowPrice,
            listing.Category.ToString(),
            listing.Region.ToString(),
            listing.Status.ToString(),
            listing.AuctionId,
            listing.CreatedAt,
            listing.UpdatedAt,
            listing.SoldAmount,
            listing.Images.Select(i => new ListingImageDto(i.Url, i.DisplayOrder)).ToList())).ToList();

        var result = PagedResult<ListingDto>.Create(dtos, pagedListings.TotalCount, pagination);
        
        return Result.Success(result);
    }
}