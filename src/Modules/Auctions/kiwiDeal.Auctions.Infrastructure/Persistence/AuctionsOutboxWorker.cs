using kiwiDeal.SharedKernel.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace kiwiDeal.Auctions.Infrastructure.Persistence;

public sealed class AuctionsOutboxWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<AuctionsOutboxWorker> logger)
    : OutboxWorker(scopeFactory, logger)
{
    protected override async Task<IReadOnlyList<OutboxMessage>> GetUnprocessedMessagesAsync(
        IServiceScope scope,
        CancellationToken cancellationToken)
    {
        var context = scope.ServiceProvider.GetRequiredService<AuctionsDbContext>();

        return await context.OutboxMessages
            .Where(m => m.ProcessedOn == null)
            .OrderBy(m => m.OccurredOn)
            .Take(20)
            .ToListAsync(cancellationToken);
    }

    protected override async Task SaveChangesAsync(
        IServiceScope scope,
        CancellationToken cancellationToken)
    {
        var context = scope.ServiceProvider.GetRequiredService<AuctionsDbContext>();
        await context.SaveChangesAsync(cancellationToken);
    }
}
