using kiwiDeal.SharedKernel.Interfaces;
using kiwiDeal.SharedKernel.Results;
using kiwiDeal.Users.Application.DTOs;
using kiwiDeal.Users.Domain.Enums;
using MediatR;

namespace kiwiDeal.Users.Application.Commands;

public sealed record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    Region Region) : IRequest<Result<AuthResponse>>, IPublicRequest;