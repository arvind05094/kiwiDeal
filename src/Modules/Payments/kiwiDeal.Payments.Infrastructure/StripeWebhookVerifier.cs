using kiwiDeal.Payments.Application;
using kiwiDeal.SharedKernel.Results;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
namespace kiwiDeal.Payments.Infrastructure;

public sealed class StripeWebhookVerifier(IOptions<StripeOptions> options) : IStripeWebhookVerifier
{
    public Result<StripeWebhookEvent> VerifyAndParse(string payload, string stripeSignature)
    {
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                payload,
                stripeSignature,
                options.Value.WebhookSecret,
                throwOnApiVersionMismatch: false);
            if (stripeEvent.Data.Object is not Session session)
                return Result.Success(new StripeWebhookEvent(string.Empty, stripeEvent.Type));
            return Result.Success(new StripeWebhookEvent(session.Id, stripeEvent.Type));
        }
        catch (StripeException ex)
        {
            return Result.Failure<StripeWebhookEvent>(Error.ValidationFailed(ex.Message));
        }
    }
}