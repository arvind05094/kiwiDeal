using kiwiDeal.Notifications.Domain.Entities;

namespace kiwiDeal.Notifications.Domain.Repositories;

public interface INotificationRepository
{
    void Add(Notification notification);

    Task<List<Notification>> GetRecentForUserAsync(
        Guid userId, int limit, CancellationToken cancellationToken = default);

    Task<int> GetUnreadCountAsync(
        Guid userId, CancellationToken cancellationToken = default);

    Task<Notification?> GetByIdAsync(
        NotificationId id, CancellationToken cancellationToken = default);

    Task<List<Notification>> GetUnreadForUserAsync(
        Guid userId, CancellationToken cancellationToken = default);
}
