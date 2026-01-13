using kiwiDeal.SharedKernel.Results;
using kiwiDeal.Users.Domain.Entities;
using kiwiDeal.Users.Domain.Errors;
using kiwiDeal.Users.Domain.Repositories;
using MediatR;

namespace kiwiDeal.Users.Application.Commands.UpdateProfile;

public sealed class UpdateProfileCommandHandler(
    IUserRepository userRepository,
    IUsersUnitOfWork unitOfWork) : IRequestHandler<UpdateProfileCommand, Result>
{
    public async Task<Result> Handle(
        UpdateProfileCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(UserId.From(request.UserId), cancellationToken);

        if (user is null)
            return Result.Failure(UserErrors.NotFound(request.UserId));

        user.UpdateProfile(request.FirstName, request.LastName, request.Region);

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
