using kiwiDeal.SharedKernel.Extensions;
using kiwiDeal.Users.Domain.Entities;
using kiwiDeal.Users.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace kiwiDeal.Users.Infrastructure.Persistence;

public sealed class UsersDbContext(DbContextOptions<UsersDbContext> options)
    : DbContext(options), IUsersUnitOfWork
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<UserRating> UserRatings => Set<UserRating>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("users");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UsersDbContext).Assembly);
        modelBuilder.ApplySoftDeleteQueryFilters();
        modelBuilder.ApplyStronglyTypedIdConverters();
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}
