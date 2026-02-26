using kiwiDeal.Messages.Domain.Repositories;

namespace kiwiDeal.Messages.Infrastructure.Persistence;

public sealed class MessagesUnitOfWork(MessagesDbContext dbContext) : IMessagesUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => dbContext.SaveChangesAsync(cancellationToken);
}
