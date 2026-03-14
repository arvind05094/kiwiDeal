using kiwiDeal.Payments.Application;
using kiwiDeal.SharedKernel.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace kiwiDeal.Payments.Infrastructure;

public sealed class StripeService(
    IOptions<StripeOptions> options,
    ILogger<StripeService> logger) : IStripeService
{
    public async Task<Result<StripeCheckoutSession>> CreateCheckoutSessionAsync(
        Guid paymentId,
        decimal amount,
        string productName,
        CancellationToken cancellationToken = default,
        Guid? listingId = null)
    {
        try
        {
            StripeConfiguration.ApiKey = options.Value.SecretKey;

            var sessionOptions = new SessionCreateOptions
            {
                PaymentMethodTypes = ["card"],
                LineItems =
                [
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "nzd",
                            UnitAmount = (long)(amount * 100),
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                               Name = productName
                            }
                        },
                        Quantity = 1
                    }
                ],
                Mode = "payment",
                SuccessUrl = listingId.HasValue
                    ? $"{options.Value.SuccessUrl}?paymentId={paymentId}&listingId={listingId.Value}"
                    : $"{options.Value.SuccessUrl}?paymentId={paymentId}",
                CancelUrl = options.Value.CancelUrl,
                Metadata = new Dictionary<string, string>
                {
                    { "payment_id", paymentId.ToString() }
                }
            };

            var service = new SessionService();
            var session = await service.CreateAsync(sessionOptions, cancellationToken: cancellationToken);

            return Result.Success(new StripeCheckoutSession(session.Id, session.Url));
        }
        catch (StripeException ex)
        {
            logger.LogWarning(ex, "Stripe error creating checkout session for payment {PaymentId}", paymentId);
            return Result.Failure<StripeCheckoutSession>(Error.Unexpected(ex.Message));
        }
    }

    public async Task<Result<string>> GetCheckoutSessionUrlAsync(
    string sessionId,
    CancellationToken cancellationToken = default)
    {
        try
        {
            StripeConfiguration.ApiKey = options.Value.SecretKey;
            var service = new SessionService();
            var session = await service.GetAsync(sessionId, cancellationToken: cancellationToken);

            if (string.IsNullOrEmpty(session.Url))
                return Result.Failure<string>(Error.Unexpected("Checkout session is no longer available."));

            return Result.Success(session.Url);
        }
        catch (StripeException ex)
        {
            logger.LogWarning(ex, "Stripe error retrieving session {SessionId}", sessionId);
            return Result.Failure<string>(Error.Unexpected(ex.Message));
        }
    }
}

public sealed class StripeOptions
{
    public const string SectionName = "Stripe";
    public string SecretKey { get; init; } = string.Empty;
    public string WebhookSecret { get; init; } = string.Empty;
    public string SuccessUrl { get; init; } = string.Empty;
    public string CancelUrl { get; init; } = string.Empty;
}
