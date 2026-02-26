using kiwiDeal.Users.Domain.Repositories;

namespace kiwiDeal.Users.Infrastructure.Persistence;

public sealed class UsersUnitOfWork(UsersDbContext dbContext) : IUsersUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => dbContext.SaveChangesAsync(cancellationToken);
}
