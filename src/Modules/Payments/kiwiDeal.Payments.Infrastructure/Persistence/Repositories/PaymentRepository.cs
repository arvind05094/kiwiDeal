using kiwiDeal.Payments.Domain.Entities;
using kiwiDeal.Payments.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace kiwiDeal.Payments.Infrastructure.Persistence.Repositories;

public sealed class PaymentRepository(PaymentsDbContext dbContext) : IPaymentRepository
{
    public async Task<Payment?> GetByIdAsync(
        PaymentId id,
        CancellationToken cancellationToken = default)
        => await dbContext.Payments
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<Payment?> GetByAuctionIdAsync(
        Guid auctionId,
        CancellationToken cancellationToken = default)
        => await dbContext.Payments
            .FirstOrDefaultAsync(p => p.AuctionId == auctionId, cancellationToken);

    public async Task<Payment?> GetByStripeSessionIdAsync(
        string stripeSessionId,
        CancellationToken cancellationToken = default)
        => await dbContext.Payments
            .FirstOrDefaultAsync(p => p.StripeSessionId == stripeSessionId, cancellationToken);

    public void Add(Payment payment)
        => dbContext.Payments.Add(payment);
}
