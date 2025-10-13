using kiwiDeal.SharedKernel.Results;
using kiwiDeal.Users.Application.DTOs;
using MediatR;

namespace kiwiDeal.Users.Application.Commands;

public sealed record RefreshTokenCommand(
    string RefreshToken) : IRequest<Result<AuthResponse>>;
