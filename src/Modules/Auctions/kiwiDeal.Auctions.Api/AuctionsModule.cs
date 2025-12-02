using kiwiDeal.Auctions.Domain.Repositories;
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

        services.AddScoped<IAuctionsUnitOfWork>(sp => sp.GetRequiredService<AuctionsDbContext>());
        services.AddScoped<IAuctionRepository, AuctionRepository>();
        services.AddScoped<IOutboxMessageProvider>(sp => sp.GetRequiredService<AuctionsDbContext>());

        return services;
    }
}
