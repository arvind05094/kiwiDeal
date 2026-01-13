using FluentValidation;

namespace kiwiDeal.Users.Application.Commands.SubmitRating;

public sealed class SubmitRatingCommandValidator : AbstractValidator<SubmitRatingCommand>
{
    public SubmitRatingCommandValidator()
    {
        RuleFor(x => x.RaterId).NotEmpty();
        RuleFor(x => x.RateeId).NotEmpty();
        RuleFor(x => x.Stars)
            .InclusiveBetween(1, 5).WithMessage("Stars must be between 1 and 5.");
        RuleFor(x => x.Comment)
            .MaximumLength(500).WithMessage("Comment must not exceed 500 characters.")
            .When(x => x.Comment is not null);
    }
}
