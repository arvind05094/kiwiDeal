using kiwiDeal.Messages.Domain.Entities;
using kiwiDeal.Messages.Domain.Repositories;
using kiwiDeal.SharedKernel.Entities;
using kiwiDeal.SharedKernel.Outbox;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace kiwiDeal.Messages.Infrastructure.Persistence;

public sealed class MessagesDbContext(DbContextOptions<MessagesDbContext> options)
    : DbContext(options), IMessagesUnitOfWork, IOutboxMessageProvider
{
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("messages");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MessagesDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        FlushDomainEventsToOutbox();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OutboxMessage>> GetUnprocessedMessagesAsync(CancellationToken cancellationToken = default)
    {
        return await OutboxMessages
            .Where(m => m.ProcessedOn == null)
            .OrderBy(m => m.OccurredOn)
            .Take(20)
            .ToListAsync(cancellationToken);
    }

    private void FlushDomainEventsToOutbox()
    {
        var aggregates = ChangeTracker
            .Entries<AggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        foreach (var aggregate in aggregates)
        {
            foreach (var domainEvent in aggregate.DomainEvents)
            {
                var payload = JsonConvert.SerializeObject(domainEvent, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.None
                });

                var outboxMessage = OutboxMessage.Create(domainEvent, payload);
                OutboxMessages.Add(outboxMessage);
            }

            aggregate.ClearDomainEvents();
        }
    }
}