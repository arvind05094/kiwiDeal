namespace kiwiDeal.Payments.Api.Requests;

public sealed record HandleWebhookRequest(
    string StripeSessionId,
    string EventType);
