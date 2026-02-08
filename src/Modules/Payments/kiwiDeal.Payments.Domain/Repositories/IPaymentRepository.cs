using kiwiDeal.Payments.Domain.Entities;

namespace kiwiDeal.Payments.Domain.Repositories;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(PaymentId id, CancellationToken cancellationToken = default);
    Task<Payment?> GetByAuctionIdAsync(Guid auctionId, CancellationToken cancellationToken = default);
    Task<Payment?> GetByStripeSessionIdAsync(string stripeSessionId, CancellationToken cancellationToken = default);
    Task<Payment?> GetByListingIdAsync(Guid listingId, CancellationToken cancellationToken = default);
    void Add(Payment payment);
    Task<(List<Payment> Items, int TotalCount)> GetFixedPriceSalesBySellerAsync(Guid sellerId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<(List<Payment> Items, int TotalCount)> GetFixedPricePurchasesByBuyerAsync(Guid buyerId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
