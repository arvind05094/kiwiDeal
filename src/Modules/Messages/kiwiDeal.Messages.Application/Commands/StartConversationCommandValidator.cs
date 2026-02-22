using FluentValidation;

namespace kiwiDeal.Messages.Application.Commands;

public class StartConversationCommandValidator : AbstractValidator<StartConversationCommand>
{
    public StartConversationCommandValidator()
    {
        RuleFor(x => x.RecipientId).NotEmpty();
        RuleFor(x => x.InitialMessage).NotEmpty().MaximumLength(2000);
    }
}
