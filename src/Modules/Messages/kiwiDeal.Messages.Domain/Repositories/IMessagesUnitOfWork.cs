namespace kiwiDeal.Messages.Domain.Repositories;

public interface IMessagesUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
