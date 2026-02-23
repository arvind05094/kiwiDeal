using kiwiDeal.Listings.Domain.Entities;
using kiwiDeal.Listings.Domain.Repositories;
using kiwiDeal.SharedKernel.Entities;
using kiwiDeal.SharedKernel.Outbox;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace kiwiDeal.Listings.Infrastructure.Persistence;

public sealed class ListingsDbContext(DbContextOptions<ListingsDbContext> options)
    : DbContext(options), IListingsUnitOfWork, IOutboxMessageProvider
{
    public DbSet<Listing> Listings => Set<Listing>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<ListingWatchlist> ListingWatchlists => Set<ListingWatchlist>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("listings");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ListingsDbContext).Assembly);
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
            .Where(m => m.ProcessedOn == null && m.RetryCount < OutboxMessage.MaxRetries)
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
