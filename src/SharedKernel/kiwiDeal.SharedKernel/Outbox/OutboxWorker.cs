using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace kiwiDeal.SharedKernel.Outbox;

public sealed class OutboxWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<OutboxWorker> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(5);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessOutboxMessagesAsync(stoppingToken);
            await Task.Delay(Interval, stoppingToken);
        }
    }

    private async Task ProcessOutboxMessagesAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();
        var registry = scope.ServiceProvider.GetRequiredService<IOutboxTypeRegistry>();
        var providers = scope.ServiceProvider.GetServices<IOutboxMessageProvider>();

        foreach (var provider in providers)
        {
            var messages = await provider.GetUnprocessedMessagesAsync(cancellationToken);

            foreach (var message in messages)
            {
                try
                {
                    var eventType = registry.Resolve(message.EventType);

                    if (eventType is null)
                    {
                        logger.LogWarning("Outbox worker could not resolve type {EventType}", message.EventType);
                        continue;
                    }

                    var domainEvent = JsonConvert.DeserializeObject(message.Payload, eventType);

                    if (domainEvent is null)
                    {
                        logger.LogWarning("Outbox worker could not deserialise message {MessageId}", message.Id);
                        continue;
                    }

                    await publisher.Publish(domainEvent, cancellationToken);

                    message.MarkProcessed(DateTimeOffset.UtcNow);

                    await provider.SaveChangesAsync(cancellationToken);

                    logger.LogInformation("Outbox message {MessageId} of type {EventType} processed successfully",
                        message.Id, message.EventType);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Outbox worker failed to process message {MessageId}. Will retry on next cycle",
                        message.Id);
                }
            }
        }
    }
}
