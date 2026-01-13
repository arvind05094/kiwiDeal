using kiwiDeal.SharedKernel.Interfaces;
using kiwiDeal.SharedKernel.Results;
using kiwiDeal.Users.Application.DTOs;
using MediatR;

namespace kiwiDeal.Users.Application.Queries.GetUserProfile;

public sealed record GetUserProfileQuery(Guid UserId) : IRequest<Result<UserProfileDto>>, IPublicRequest;
