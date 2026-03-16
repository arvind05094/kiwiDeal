using kiwiDeal.Notifications.Application.Events;
using kiwiDeal.Notifications.Domain.Repositories;
using kiwiDeal.Notifications.Infrastructure.Persistence;
using kiwiDeal.Notifications.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace kiwiDeal.Notifications.Api;

public static class NotificationsModule
{
    public static IServiceCollection AddNotificationsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<NotificationsDbContext>(options =>
            options
                .UseNpgsql(configuration.GetConnectionString("NotificationsConnection"))
                .UseSnakeCaseNamingConvention());

        services.AddScoped<INotificationsUnitOfWork>(sp => sp.GetRequiredService<NotificationsDbContext>());
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationHubContext, NotificationHubContext>();

        return services;
    }
}
