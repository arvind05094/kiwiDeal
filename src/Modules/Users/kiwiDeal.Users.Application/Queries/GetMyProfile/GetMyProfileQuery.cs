using kiwiDeal.SharedKernel.Results;
using kiwiDeal.Users.Application.DTOs;
using MediatR;

namespace kiwiDeal.Users.Application.Queries.GetMyProfile;

public sealed record GetMyProfileQuery(Guid UserId) : IRequest<Result<MyProfileDto>>;
