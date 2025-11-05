using kiwiDeal.Listings.Domain.Repositories;
using kiwiDeal.Listings.Infrastructure;
using kiwiDeal.Listings.Infrastructure.Persistence;
using kiwiDeal.Listings.Infrastructure.Persistence.Repositories;
using kiwiDeal.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace kiwiDeal.Listings.Api;

public static class ListingsModule
{
    public static IServiceCollection AddListingsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ListingsDbContext>(options =>
            options
                .UseNpgsql(configuration.GetConnectionString("ListingsConnection"))
                .UseSnakeCaseNamingConvention());

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ListingsDbContext>());
        services.AddScoped<IListingRepository, ListingRepository>();
        services.AddScoped<IImageService, AzureBlobImageService>();

        return services;
    }
}
