using kiwiDeal.SharedKernel.Interfaces;
using kiwiDeal.SharedKernel.Results;
using kiwiDeal.Users.Application.DTOs;
using MediatR;

namespace kiwiDeal.Users.Application.Commands;

public sealed record LoginCommand(
    string Email,
    string Password) : IRequest<Result<AuthResponse>>, IPublicRequest;
