using kiwiDeal.Payments.Domain.Errors;
using kiwiDeal.Payments.Domain.Repositories;
using kiwiDeal.SharedKernel.Interfaces;
using kiwiDeal.SharedKernel.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace kiwiDeal.Payments.Application.Commands.CreateCheckoutSession;

public sealed class CreateCheckoutSessionCommandHandler(
    IPaymentRepository paymentRepository,
    IStripeService stripeService,
    IPaymentsUnitOfWork unitOfWork,
    ILogger<CreateCheckoutSessionCommandHandler> logger) : IRequestHandler<CreateCheckoutSessionCommand, Result<string>>
{
    public async Task<Result<string>> Handle(
        CreateCheckoutSessionCommand request,
        CancellationToken cancellationToken)
    {
        var payment = await paymentRepository.GetByAuctionIdAsync(
            request.AuctionId, cancellationToken);

        if (payment is null)
            return Result.Failure<string>(PaymentErrors.NotFound);

        if (payment.Status == Domain.Entities.PaymentStatus.Completed)
            return Result.Failure<string>(PaymentErrors.AlreadyProcessed);

        var sessionResult = await stripeService.CreateCheckoutSessionAsync(
            payment.Id.Value,
            payment.Amount,
            cancellationToken);

        if (sessionResult.IsFailure)
            return Result.Failure<string>(sessionResult.Error);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Stripe checkout session created for payment {PaymentId}",
            payment.Id);

        return Result.Success(sessionResult.Value);
    }
}
