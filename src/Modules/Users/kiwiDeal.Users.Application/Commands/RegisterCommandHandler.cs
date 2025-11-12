using kiwiDeal.SharedKernel.Results;
using kiwiDeal.Users.Application.DTOs;
using kiwiDeal.Users.Domain.Entities;
using kiwiDeal.Users.Domain.Errors;
using kiwiDeal.Users.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace kiwiDeal.Users.Application.Commands;

public sealed class RegisterCommandHandler(
    IUserRepository userRepository,
    IUsersUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    ILogger<RegisterCommandHandler> logger) : IRequestHandler<RegisterCommand, Result<UserResponse>>
{
    public async Task<Result<UserResponse>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Registering user with email {Email}", request.Email);

        var existing = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (existing is not null)
        {
            logger.LogWarning("Registration failed. Email {Email} already in use", request.Email);
            return Result.Failure<UserResponse>(UserErrors.EmailAlreadyInUse);
        }

        var passwordHash = passwordHasher.Hash(request.Password);

        var result = User.Create(
            request.Email,
            passwordHash,
            request.FirstName,
            request.LastName);

        if (result.IsFailure)
            return Result.Failure<UserResponse>(result.Error);

        var user = result.Value;

        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User {UserId} registered successfully", user.Id);

        return Result.Success(new UserResponse(
            user.Id.Value,
            user.Email,
            user.FirstName,
            user.LastName));
    }
}
