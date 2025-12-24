using kiwiDeal.SharedKernel.Results;
namespace kiwiDeal.Payments.Application;

public record StripeCheckoutSession(string SessionId, string Url);

public interface IStripeService
{
    Task<Result<StripeCheckoutSession>> CreateCheckoutSessionAsync(
        Guid paymentId,
        decimal amount,
        CancellationToken cancellationToken = default);
}