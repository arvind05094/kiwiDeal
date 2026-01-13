using kiwiDeal.SharedKernel.Results;
using kiwiDeal.Users.Application.DTOs;
using kiwiDeal.Users.Domain.Entities;
using kiwiDeal.Users.Domain.Errors;
using kiwiDeal.Users.Domain.Repositories;
using MediatR;

namespace kiwiDeal.Users.Application.Queries.GetUserProfile;

public sealed class GetUserProfileQueryHandler(
    IUserRepository userRepository) : IRequestHandler<GetUserProfileQuery, Result<UserProfileDto>>
{
    public async Task<Result<UserProfileDto>> Handle(
        GetUserProfileQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(UserId.From(request.UserId), cancellationToken);

        if (user is null)
            return Result.Failure<UserProfileDto>(UserErrors.NotFound(request.UserId));

        var ratings = await userRepository.GetRatingsByRateeAsync(user.Id, cancellationToken);

        var averageRating = ratings.Count > 0
            ? (double?)ratings.Average(r => r.Stars)
            : null;

        return Result.Success(new UserProfileDto(
            user.Id.Value,
            user.FirstName,
            user.LastName,
            user.Region.ToString(),
            user.CreatedAt,
            averageRating,
            ratings.Count));
    }
}
