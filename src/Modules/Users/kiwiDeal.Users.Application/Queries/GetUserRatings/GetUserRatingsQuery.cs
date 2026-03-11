using kiwiDeal.SharedKernel.Interfaces;
using kiwiDeal.SharedKernel.Pagination;
using kiwiDeal.SharedKernel.Results;
using kiwiDeal.Users.Application.DTOs;
using MediatR;

namespace kiwiDeal.Users.Application.Queries.GetUserRatings;

public sealed record GetUserRatingsQuery(Guid UserId, int PageNumber, int PageSize)
    : IRequest<Result<PagedResult<UserRatingDto>>>, IPublicRequest;