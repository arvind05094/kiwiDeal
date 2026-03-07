using kiwiDeal.SharedKernel.Results;
namespace kiwiDeal.Payments.Application;

public record StripeCheckoutSession(string SessionId, string Url);

public interface IStripeService
{
    Task<Result<StripeCheckoutSession>> CreateCheckoutSessionAsync(
        Guid paymentId,
        decimal amount,
        string productName,
        CancellationToken cancellationToken = default,
        Guid? listingId = null);

    Task<Result<string>> GetCheckoutSessionUrlAsync(
        string sessionId,
        CancellationToken cancellationToken = default);
}