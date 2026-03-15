using kiwiDeal.Notifications.Domain.Entities;
using kiwiDeal.Notifications.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace kiwiDeal.Notifications.Infrastructure.Persistence.Repositories;

public class NotificationRepository(NotificationsDbContext context) : INotificationRepository
{
    public void Add(Notification notification)
    {
        context.Notifications.Add(notification);
    }

    public async Task<List<Notification>> GetRecentForUserAsync(
        Guid userId, int limit, CancellationToken cancellationToken = default)
    {
        return await context.Notifications
            .Where(n => n.RecipientId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetUnreadCountAsync(
        Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.Notifications
            .Where(n => n.RecipientId == userId && !n.IsRead)
            .CountAsync(cancellationToken);
    }

    public async Task<Notification?> GetByIdAsync(
        NotificationId id, CancellationToken cancellationToken = default)
    {
        return await context.Notifications
            .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
    }

    public async Task<List<Notification>> GetUnreadForUserAsync(
        Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.Notifications
            .Where(n => n.RecipientId == userId && !n.IsRead)
            .ToListAsync(cancellationToken);
    }
}
