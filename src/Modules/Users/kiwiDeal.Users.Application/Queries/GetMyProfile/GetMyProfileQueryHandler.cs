using kiwiDeal.SharedKernel.Results;
using kiwiDeal.Users.Application.DTOs;
using kiwiDeal.Users.Domain.Entities;
using kiwiDeal.Users.Domain.Errors;
using kiwiDeal.Users.Domain.Repositories;
using MediatR;

namespace kiwiDeal.Users.Application.Queries.GetMyProfile;

public sealed class GetMyProfileQueryHandler(
    IUserRepository userRepository) : IRequestHandler<GetMyProfileQuery, Result<MyProfileDto>>
{
    public async Task<Result<MyProfileDto>> Handle(
        GetMyProfileQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(UserId.From(request.UserId), cancellationToken);

        if (user is null)
            return Result.Failure<MyProfileDto>(UserErrors.NotFound(request.UserId));

        return Result.Success(new MyProfileDto(
            user.Id.Value,
            user.Email,
            user.FirstName,
            user.LastName,
            user.Region.ToString(),
            user.CreatedAt));
    }
}
