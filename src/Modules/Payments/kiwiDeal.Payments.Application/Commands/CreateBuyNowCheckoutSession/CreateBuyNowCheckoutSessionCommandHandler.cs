using kiwiDeal.Payments.Domain.Entities;
using kiwiDeal.Payments.Domain.Errors;
using kiwiDeal.Payments.Domain.Repositories;
using kiwiDeal.SharedKernel.Interfaces;
using kiwiDeal.SharedKernel.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace kiwiDeal.Payments.Application.Commands.CreateBuyNowCheckoutSession;

public sealed class CreateBuyNowCheckoutSessionCommandHandler(
    IPaymentRepository paymentRepository,
    IStripeService stripeService,
    IPaymentsUnitOfWork unitOfWork,
    ILogger<CreateBuyNowCheckoutSessionCommandHandler> logger)
    : IRequestHandler<CreateBuyNowCheckoutSessionCommand, Result<string>>
{
    public async Task<Result<string>> Handle(
        CreateBuyNowCheckoutSessionCommand request,
        CancellationToken cancellationToken)
    {
        // Check no active payment already exists for this listing
        var existing = await paymentRepository.GetByListingIdAsync(
            request.ListingId, cancellationToken);

        if (existing is not null && existing.Status == PaymentStatus.Pending)
        {
            if (!string.IsNullOrEmpty(existing.StripeSessionId))
            {
                var urlResult = await stripeService.GetCheckoutSessionUrlAsync(
                    existing.StripeSessionId, cancellationToken);
                if (urlResult.IsSuccess)
                    return Result.Success(urlResult.Value);
            }
            return Result.Failure<string>(PaymentErrors.ActiveSessionExists);
        }
        if (existing is not null && existing.Status == PaymentStatus.Completed)
            return Result.Failure<string>(PaymentErrors.AlreadyProcessed);

        // Amount and seller come from the listing — passed in via command
        // The controller reads listing details before dispatching this command
        var paymentResult = Payment.Create(
            auctionId: null,
            request.ListingId,
            request.BuyerId,
            request.SellerId,
            request.Amount,
            "FixedPrice");

        if (paymentResult.IsFailure)
            return Result.Failure<string>(paymentResult.Error);

        var sessionResult = await stripeService.CreateCheckoutSessionAsync(
            paymentResult.Value.Id.Value,
            paymentResult.Value.Amount,
            "kiwiDeal Buy Now",
            cancellationToken,
            listingId: request.ListingId);

        if (sessionResult.IsFailure)
            return Result.Failure<string>(sessionResult.Error);

        paymentResult.Value.SetStripeSessionId(sessionResult.Value.SessionId);
        paymentRepository.Add(paymentResult.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Buy now checkout session created for listing {ListingId} buyer {BuyerId}",
            request.ListingId, request.BuyerId);

        return Result.Success(sessionResult.Value.Url);
    }
}