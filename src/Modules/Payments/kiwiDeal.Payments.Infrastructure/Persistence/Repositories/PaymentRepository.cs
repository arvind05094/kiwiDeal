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
    public async Task<Payment?> GetByListingIdAsync(
    Guid listingId,
    CancellationToken cancellationToken = default)
    => await dbContext.Payments
        .Where(p => p.ListingId == listingId)
        .OrderByDescending(p => p.CreatedAt)
        .FirstOrDefaultAsync(cancellationToken);
    public void Add(Payment payment)
        => dbContext.Payments.Add(payment);

    public async Task<(List<Payment> Items, int TotalCount)> GetFixedPriceSalesBySellerAsync(
    Guid sellerId, int pageNumber, int pageSize,
    CancellationToken cancellationToken = default)
    {
        var query = dbContext.Payments
            .Where(p => p.SellerId == sellerId && p.PaymentType == "FixedPrice");
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return (items, totalCount);
    }

    public async Task<(List<Payment> Items, int TotalCount)> GetFixedPricePurchasesByBuyerAsync(
        Guid buyerId, int pageNumber, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Payments
            .Where(p => p.BuyerId == buyerId && p.PaymentType == "FixedPrice");
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return (items, totalCount);
    }
}
