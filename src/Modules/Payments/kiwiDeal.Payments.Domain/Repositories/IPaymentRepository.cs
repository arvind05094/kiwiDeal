using kiwiDeal.Payments.Domain.Entities;

namespace kiwiDeal.Payments.Domain.Repositories;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(PaymentId id, CancellationToken cancellationToken = default);
    Task<Payment?> GetByAuctionIdAsync(Guid auctionId, CancellationToken cancellationToken = default);
    void Add(Payment payment);
}
