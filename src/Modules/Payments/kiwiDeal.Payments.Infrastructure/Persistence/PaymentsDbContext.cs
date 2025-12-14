using kiwiDeal.Payments.Domain.Entities;
using kiwiDeal.Payments.Domain.Repositories;
using kiwiDeal.SharedKernel.Outbox;
using Microsoft.EntityFrameworkCore;

namespace kiwiDeal.Payments.Infrastructure.Persistence;

public sealed class PaymentsDbContext(DbContextOptions<PaymentsDbContext> options)
    : DbContext(options), IPaymentsUnitOfWork, IOutboxMessageProvider
{
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("payments");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaymentsDbContext).Assembly);
    }

    public new async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await base.SaveChangesAsync(cancellationToken);

    public async Task<IReadOnlyList<OutboxMessage>> GetUnprocessedMessagesAsync(
        CancellationToken cancellationToken = default)
        => await OutboxMessages
            .Where(m => m.ProcessedOn == null)
            .OrderBy(m => m.OccurredOn)
            .Take(50)
            .ToListAsync(cancellationToken);
}
