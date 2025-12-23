using kiwiDeal.SharedKernel.Results;

namespace kiwiDeal.Payments.Application;

public record StripeWebhookEvent(string SessionId, string EventType);

public interface IStripeWebhookVerifier
{
    Result<StripeWebhookEvent> VerifyAndParse(string payload, string stripeSignature);
}
