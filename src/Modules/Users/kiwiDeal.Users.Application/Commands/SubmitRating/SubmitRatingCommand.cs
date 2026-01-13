using kiwiDeal.SharedKernel.Results;
using MediatR;

namespace kiwiDeal.Users.Application.Commands.SubmitRating;

public sealed record SubmitRatingCommand(
    Guid RaterId,
    Guid RateeId,
    int Stars,
    string? Comment) : IRequest<Result>;
