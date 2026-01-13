using kiwiDeal.SharedKernel.Results;
using kiwiDeal.Users.Domain.Entities;
using kiwiDeal.Users.Domain.Errors;
using kiwiDeal.Users.Domain.Repositories;
using MediatR;

namespace kiwiDeal.Users.Application.Commands.SubmitRating;

public sealed class SubmitRatingCommandHandler(
    IUserRepository userRepository,
    IUsersUnitOfWork unitOfWork) : IRequestHandler<SubmitRatingCommand, Result>
{
    public async Task<Result> Handle(
        SubmitRatingCommand request,
        CancellationToken cancellationToken)
    {
        var ratee = await userRepository.GetByIdAsync(UserId.From(request.RateeId), cancellationToken);

        if (ratee is null)
            return Result.Failure(UserErrors.NotFound(request.RateeId));

        var existing = await userRepository.GetRatingAsync(
            UserId.From(request.RaterId),
            UserId.From(request.RateeId),
            cancellationToken);

        if (existing is not null)
            return Result.Failure(UserErrors.AlreadyRated);

        var rating = UserRating.Create(
            UserId.From(request.RaterId),
            UserId.From(request.RateeId),
            request.Stars,
            request.Comment);

        await userRepository.AddRatingAsync(rating, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
