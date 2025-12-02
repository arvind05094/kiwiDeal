namespace kiwiDeal.SharedKernel.Outbox;

public interface IOutboxMessageProvider
{
    Task<IReadOnlyList<OutboxMessage>> GetUnprocessedMessagesAsync(CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
