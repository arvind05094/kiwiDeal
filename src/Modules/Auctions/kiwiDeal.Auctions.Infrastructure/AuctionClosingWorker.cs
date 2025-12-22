using kiwiDeal.Auctions.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace kiwiDeal.Auctions.Infrastructure;

public sealed class AuctionClosingWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<AuctionClosingWorker> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(30);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("AuctionClosingWorker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CloseExpiredAuctionsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "AuctionClosingWorker encountered an error");
            }

            await Task.Delay(Interval, stoppingToken);
        }
    }

    private async Task CloseExpiredAuctionsAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IAuctionRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IAuctionsUnitOfWork>();

        var expiredAuctions = await repository.GetExpiredActiveAuctionsAsync(cancellationToken);

        if (expiredAuctions.Count == 0)
            return;

        logger.LogInformation("Closing {Count} expired auctions", expiredAuctions.Count);

        foreach (var auction in expiredAuctions)
        {
            var result = auction.Close();
            if (result.IsFailure)
            {
                logger.LogWarning("Failed to close auction {AuctionId}: {Error}", auction.Id, result.Error);
                continue;
            }

            logger.LogInformation("Auction {AuctionId} closed. Winner: {WinnerId}", auction.Id, auction.CurrentHighestBidderId);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
