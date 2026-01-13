using FluentValidation;

namespace kiwiDeal.Payments.Application.Commands.CreateBuyNowCheckoutSession;

public sealed class CreateBuyNowCheckoutSessionCommandValidator
    : AbstractValidator<CreateBuyNowCheckoutSessionCommand>
{
    public CreateBuyNowCheckoutSessionCommandValidator()
    {
        RuleFor(x => x.ListingId).NotEmpty();
        RuleFor(x => x.BuyerId).NotEmpty();
    }
}