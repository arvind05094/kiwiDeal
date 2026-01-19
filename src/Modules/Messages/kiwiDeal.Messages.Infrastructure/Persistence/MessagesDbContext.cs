using kiwiDeal.Messages.Domain.Entities;
using kiwiDeal.Messages.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace kiwiDeal.Messages.Infrastructure.Persistence;

public sealed class MessagesDbContext(DbContextOptions<MessagesDbContext> options)
    : DbContext(options), IMessagesUnitOfWork
{
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<Message> Messages => Set<Message>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("messages");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MessagesDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}
