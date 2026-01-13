using kiwiDeal.SharedKernel.Results;
using kiwiDeal.Users.Domain.Enums;
using MediatR;

namespace kiwiDeal.Users.Application.Commands.UpdateProfile;

public sealed record UpdateProfileCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    Region Region) : IRequest<Result>;
