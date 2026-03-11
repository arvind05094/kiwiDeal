using kiwiDeal.SharedKernel.Pagination;
using kiwiDeal.SharedKernel.Results;
using kiwiDeal.Users.Application.DTOs;
using kiwiDeal.Users.Domain.Entities;
using kiwiDeal.Users.Domain.Errors;
using kiwiDeal.Users.Domain.Repositories;
using MediatR;

namespace kiwiDeal.Users.Application.Queries.GetUserRatings;

public sealed class GetUserRatingsQueryHandler(
    IUserRepository userRepository) : IRequestHandler<GetUserRatingsQuery, Result<PagedResult<UserRatingDto>>>
{
    public async Task<Result<PagedResult<UserRatingDto>>> Handle(
        GetUserRatingsQuery request,
        CancellationToken cancellationToken)
    {
        var ratee = await userRepository.GetByIdAsync(UserId.From(request.UserId), cancellationToken);
        if (ratee is null)
            return Result.Failure<PagedResult<UserRatingDto>>(UserErrors.NotFound(request.UserId));

        var pagination = new PaginationParams(request.PageNumber, request.PageSize);
        var (ratings, total) = await userRepository.GetPagedRatingsByRateeAsync(
            UserId.From(request.UserId), pagination.Skip, pagination.PageSize, cancellationToken);

        // Batch-load rater names
        var raterIds = ratings.Select(r => r.RaterId).Distinct().ToList();
        var raters = new Dictionary<UserId, string>();
        foreach (var raterId in raterIds)
        {
            var rater = await userRepository.GetByIdAsync(raterId, cancellationToken);
            if (rater is not null)
                raters[raterId] = $"{rater.FirstName} {rater.LastName[0]}.";
        }

        var dtos = ratings.Select(r => new UserRatingDto(
            r.Stars,
            r.Comment,
            raters.GetValueOrDefault(r.RaterId, "User"),
            r.CreatedAt)).ToList();

        return Result.Success(PagedResult<UserRatingDto>.Create(dtos, total, pagination));
    }
}