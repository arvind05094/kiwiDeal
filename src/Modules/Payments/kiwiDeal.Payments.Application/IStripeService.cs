using kiwiDeal.SharedKernel.Results;

namespace kiwiDeal.Payments.Application;

public interface IStripeService
{
    Task<Result<string>> CreateCheckoutSessionAsync(
        Guid paymentId,
        decimal amount,
        CancellationToken cancellationToken = default);
}
