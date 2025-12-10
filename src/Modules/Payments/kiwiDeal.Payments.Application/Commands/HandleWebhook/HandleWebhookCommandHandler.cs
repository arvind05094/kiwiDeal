using kiwiDeal.Payments.Domain.Errors;
using kiwiDeal.Payments.Domain.Repositories;
using kiwiDeal.SharedKernel.Interfaces;
using kiwiDeal.SharedKernel.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace kiwiDeal.Payments.Application.Commands.HandleWebhook;

public sealed class HandleWebhookCommandHandler(
    IPaymentRepository paymentRepository,
    IPaymentsUnitOfWork unitOfWork,
    ILogger<HandleWebhookCommandHandler> logger) : IRequestHandler<HandleWebhookCommand, Result>
{
    private const string CheckoutSessionCompleted = "checkout.session.completed";
    private const string CheckoutSessionExpired = "checkout.session.expired";

    public async Task<Result> Handle(
        HandleWebhookCommand request,
        CancellationToken cancellationToken)
    {
        var payment = await paymentRepository.GetByStripeSessionIdAsync(
            request.StripeSessionId, cancellationToken);

        if (payment is null)
            return Result.Failure(PaymentErrors.NotFound);

        var result = request.EventType switch
        {
            CheckoutSessionCompleted => payment.Complete(request.StripeSessionId),
            CheckoutSessionExpired => payment.Fail(),
            _ => Result.Success()
        };

        if (result.IsFailure)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Webhook {EventType} processed for payment {PaymentId}",
            request.EventType, payment.Id);

        return Result.Success();
    }
}
