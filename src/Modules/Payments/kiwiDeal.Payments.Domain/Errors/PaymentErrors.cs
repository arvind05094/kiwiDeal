using kiwiDeal.SharedKernel.Results;

namespace kiwiDeal.Payments.Domain.Errors;

public static class PaymentErrors
{
    public static readonly Error AlreadyProcessed = Error.PaymentAlreadyProcessed(
        "Payment has already been processed.");

    public static readonly Error NotFound = Error.NotFound(
        "Payment was not found.");
}
