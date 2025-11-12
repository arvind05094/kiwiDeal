using kiwiDeal.SharedKernel.Results;
using kiwiDeal.Users.Application.DTOs;
using kiwiDeal.Users.Domain.Errors;
using kiwiDeal.Users.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace kiwiDeal.Users.Application.Commands;

public sealed class LoginCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator,
    IUsersUnitOfWork unitOfWork,
    ILogger<LoginCommandHandler> logger) : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Login attempt for email {Email}", request.Email);

        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            logger.LogWarning("Login failed for email {Email}", request.Email);
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
        }

        var accessToken = jwtTokenGenerator.GenerateAccessToken(user);
        var refreshTokenValue = jwtTokenGenerator.GenerateRefreshToken();
        user.AddRefreshToken(refreshTokenValue, DateTimeOffset.UtcNow.AddDays(7));

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User {UserId} logged in successfully", user.Id);

        return Result.Success(new AuthResponse(
            accessToken,
            refreshTokenValue,
            new UserResponse(
                user.Id.Value,
                user.Email,
                user.FirstName,
                user.LastName)));
    }
}
