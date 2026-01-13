using kiwiDeal.Payments.Domain.Errors;
using kiwiDeal.Payments.Domain.Events;
using kiwiDeal.SharedKernel.Entities;
using kiwiDeal.SharedKernel.Results;

namespace kiwiDeal.Payments.Domain.Entities;

public sealed class Payment : AggregateRoot
{
    public PaymentId Id { get; private set; } = default!;
    public Guid? AuctionId { get; private set; }
    public Guid ListingId { get; private set; }
    public Guid BuyerId { get; private set; }
    public Guid SellerId { get; private set; }
    public decimal Amount { get; private set; }
    public string PaymentType { get; private set; } = default!;
    public PaymentStatus Status { get; private set; }
    public string? StripeSessionId { get; private set; }
    public DateTimeOffset? PaidAt { get; private set; }

    private Payment() { }

    public static Result<Payment> Create(
        Guid? auctionId,
        Guid listingId,
        Guid buyerId,
        Guid sellerId,
        decimal amount,
        string paymentType)
    {
        var now = DateTimeOffset.UtcNow;
        var payment = new Payment
        {
            Id = PaymentId.New(),
            AuctionId = auctionId,
            ListingId = listingId,
            BuyerId = buyerId,
            SellerId = sellerId,
            Amount = amount,
            PaymentType = paymentType,
            Status = PaymentStatus.Pending,
            CreatedAt = now,
            UpdatedAt = now
        };

        return Result.Success(payment);
    }

    public Result SetStripeSessionId(string stripeSessionId)
    {
        if (string.IsNullOrWhiteSpace(stripeSessionId))
            return Result.Failure(Error.ValidationFailed("Stripe session ID cannot be empty."));
        StripeSessionId = stripeSessionId;
        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }

    public Result Complete(string stripeSessionId)
    {
        if (Status == PaymentStatus.Completed)
            return Result.Failure(PaymentErrors.AlreadyProcessed);

        Status = PaymentStatus.Completed;
        StripeSessionId = stripeSessionId;
        PaidAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;

        RaiseDomainEvent(new PaymentCompletedEvent(
            Id.Value,
            AuctionId,
            ListingId,
            BuyerId,
            SellerId,
            Amount,
            PaymentType));

        return Result.Success();
    }

    public Result Fail()
    {
        if (Status == PaymentStatus.Completed)
            return Result.Failure(PaymentErrors.AlreadyProcessed);

        Status = PaymentStatus.Failed;
        UpdatedAt = DateTimeOffset.UtcNow;

        RaiseDomainEvent(new PaymentFailedEvent(Id.Value, AuctionId, BuyerId));

        return Result.Success();
    }
}
