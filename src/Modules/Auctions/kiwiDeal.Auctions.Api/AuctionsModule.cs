using kiwiDeal.Auctions.Domain.Repositories;
using kiwiDeal.Auctions.Infrastructure;
using kiwiDeal.Auctions.Infrastructure.Persistence;
using kiwiDeal.Auctions.Infrastructure.Persistence.Repositories;
using kiwiDeal.SharedKernel.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace kiwiDeal.Auctions.Api;

public static class AuctionsModule
{
    public static IServiceCollection AddAuctionsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AuctionsDbContext>(options =>
            options
                .UseNpgsql(configuration.GetConnectionString("AuctionsConnection"))
                .UseSnakeCaseNamingConvention());
        services.AddScoped<IAuctionsUnitOfWork, AuctionsUnitOfWork>();
        services.AddScoped<IAuctionRepository, AuctionRepository>();
        services.AddScoped<IAuctionWatchlistRepository, AuctionWatchlistRepository>();
        services.AddScoped<IOutboxMessageProvider>(sp => sp.GetRequiredService<AuctionsDbContext>());
        services.AddHostedService<AuctionActivationWorker>();
        services.AddHostedService<AuctionClosingWorker>();
        return services;
    }
}
