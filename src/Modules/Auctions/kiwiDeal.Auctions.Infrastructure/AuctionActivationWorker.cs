using kiwiDeal.Auctions.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace kiwiDeal.Auctions.Infrastructure;

public sealed class AuctionActivationWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<AuctionActivationWorker> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(30);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("AuctionActivationWorker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ActivateScheduledAuctionsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "AuctionActivationWorker encountered an error");
            }

            await Task.Delay(Interval, stoppingToken);
        }
    }

    private async Task ActivateScheduledAuctionsAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IAuctionRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IAuctionsUnitOfWork>();

        var auctions = await repository.GetScheduledReadyToActivateAsync(cancellationToken);

        if (auctions.Count == 0)
            return;

        logger.LogInformation("Activating {Count} scheduled auctions", auctions.Count);

        foreach (var auction in auctions)
        {
            var result = auction.Activate();
            if (result.IsFailure)
            {
                logger.LogWarning("Failed to activate auction {AuctionId}: {Error}", auction.Id, result.Error);
                continue;
            }

            logger.LogInformation("Auction {AuctionId} activated", auction.Id);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}