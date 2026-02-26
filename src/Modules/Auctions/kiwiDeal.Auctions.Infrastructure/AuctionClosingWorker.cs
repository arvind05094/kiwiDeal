using kiwiDeal.Auctions.Application.Commands;
using kiwiDeal.Auctions.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
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
        var hubContext = scope.ServiceProvider.GetRequiredService<IAuctionHubContext>();

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

            try
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
                logger.LogInformation("Auction {AuctionId} closed. Winner: {WinnerId}", auction.Id, auction.CurrentHighestBidderId);
                await hubContext.SendAuctionClosed(auction.Id.Value.ToString(), auction.CurrentHighestBidderId, auction.CurrentHighestBid, cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                logger.LogInformation("Auction {AuctionId} was already closed by another instance", auction.Id);
            }
        }
    }
}
