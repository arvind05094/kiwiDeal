using FluentAssertions;
using kiwiDeal.Payments.Domain.Entities;
using kiwiDeal.SharedKernel.Results;

namespace kiwiDeal.Tests.Unit.Payments.Domain;

public class PaymentTests
{
    private static Payment CreatePendingPayment(
        Guid? auctionId = null,
        Guid? winnerId = null,
        Guid? sellerId = null,
        decimal amount = 100m)
    {
        return Payment.Create(
            auctionId ?? Guid.NewGuid(),
            winnerId ?? Guid.NewGuid(),
            sellerId ?? Guid.NewGuid(),
            amount).Value;
    }

    [Fact]
    public void Create_ValidInput_ReturnsSuccess()
    {
        var auctionId = Guid.NewGuid();
        var winnerId = Guid.NewGuid();
        var sellerId = Guid.NewGuid();

        var result = Payment.Create(auctionId, winnerId, sellerId, 250m);

        result.IsSuccess.Should().BeTrue();
        result.Value.AuctionId.Should().Be(auctionId);
        result.Value.WinnerId.Should().Be(winnerId);
        result.Value.SellerId.Should().Be(sellerId);
        result.Value.Amount.Should().Be(250m);
        result.Value.Status.Should().Be(PaymentStatus.Pending);
        result.Value.StripeSessionId.Should().BeNull();
        result.Value.PaidAt.Should().BeNull();
    }

    [Fact]
    public void Complete_PendingPayment_ReturnsSuccess()
    {
        var payment = CreatePendingPayment();

        var result = payment.Complete("cs_test_abc123");

        result.IsSuccess.Should().BeTrue();
        payment.Status.Should().Be(PaymentStatus.Completed);
        payment.StripeSessionId.Should().Be("cs_test_abc123");
        payment.PaidAt.Should().NotBeNull();
    }

    [Fact]
    public void Complete_AlreadyCompleted_ReturnsFailure()
    {
        var payment = CreatePendingPayment();
        payment.Complete("cs_test_abc123");

        var result = payment.Complete("cs_test_xyz789");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(ErrorCode.PaymentAlreadyProcessed);
    }

    [Fact]
    public void Complete_RaisesPaymentCompletedEvent()
    {
        var payment = CreatePendingPayment();

        payment.Complete("cs_test_abc123");

        payment.DomainEvents.Should().HaveCount(1);
        payment.DomainEvents[0].Should().BeOfType<kiwiDeal.Payments.Domain.Events.PaymentCompletedEvent>();
    }

    [Fact]
    public void Fail_PendingPayment_ReturnsSuccess()
    {
        var payment = CreatePendingPayment();

        var result = payment.Fail();

        result.IsSuccess.Should().BeTrue();
        payment.Status.Should().Be(PaymentStatus.Failed);
    }

    [Fact]
    public void Fail_AlreadyCompleted_ReturnsFailure()
    {
        var payment = CreatePendingPayment();
        payment.Complete("cs_test_abc123");

        var result = payment.Fail();

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(ErrorCode.PaymentAlreadyProcessed);
    }

    [Fact]
    public void Fail_RaisesPaymentFailedEvent()
    {
        var payment = CreatePendingPayment();

        payment.Fail();

        payment.DomainEvents.Should().HaveCount(1);
        payment.DomainEvents[0].Should().BeOfType<kiwiDeal.Payments.Domain.Events.PaymentFailedEvent>();
    }
}
