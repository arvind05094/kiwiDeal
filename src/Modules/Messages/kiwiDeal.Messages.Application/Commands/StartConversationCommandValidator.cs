using FluentValidation;

namespace kiwiDeal.Messages.Application.Commands;

public class StartConversationCommandValidator : AbstractValidator<StartConversationCommand>
{
    public StartConversationCommandValidator()
    {
        RuleFor(x => x.RecipientId).NotEmpty();
        RuleFor(x => x.InitialMessage).MaximumLength(2000).When(x => x.InitialMessage != null);
    }
}
