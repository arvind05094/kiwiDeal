using kiwiDeal.SharedKernel.Results;
using kiwiDeal.Users.Application.DTOs;
using kiwiDeal.Users.Domain.Errors;
using kiwiDeal.Users.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace kiwiDeal.Users.Application.Commands;

public sealed class RefreshTokenCommandHandler(
    IUserRepository userRepository,
    IJwtTokenGenerator jwtTokenGenerator,
    IUsersUnitOfWork unitOfWork,
    ILogger<RefreshTokenCommandHandler> logger) : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Refresh token attempt");

        var user = await userRepository.GetByRefreshTokenAsync(request.RefreshToken, cancellationToken);

        if (user is null)
        {
            logger.LogWarning("Refresh token not found");
            return Result.Failure<AuthResponse>(UserErrors.InvalidRefreshToken);
        }

        var activeTokenResult = user.GetActiveRefreshToken(request.RefreshToken);

        if (activeTokenResult.IsFailure)
        {
            logger.LogWarning("Refresh token is invalid or expired for user {UserId}", user.Id);
            return Result.Failure<AuthResponse>(activeTokenResult.Error);
        }

        activeTokenResult.Value.Revoke();

        var accessToken = jwtTokenGenerator.GenerateAccessToken(user);
        var newRefreshTokenValue = jwtTokenGenerator.GenerateRefreshToken();
        user.AddRefreshToken(newRefreshTokenValue, DateTimeOffset.UtcNow.AddDays(7));

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Refresh token rotated successfully for user {UserId}", user.Id);

        return Result.Success(new AuthResponse(
            accessToken,
            newRefreshTokenValue,
            new UserResponse(
                user.Id.Value,
                user.Email,
                user.FirstName,
                user.LastName,
                user.Role)));
    }
}
