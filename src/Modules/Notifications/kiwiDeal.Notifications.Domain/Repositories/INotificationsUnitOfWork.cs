namespace kiwiDeal.Notifications.Domain.Repositories;

public interface INotificationsUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
