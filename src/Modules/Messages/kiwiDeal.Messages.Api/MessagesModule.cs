using kiwiDeal.Messages.Application.Commands;
using kiwiDeal.Messages.Domain.Repositories;
using kiwiDeal.Messages.Infrastructure.Persistence;
using kiwiDeal.Messages.Infrastructure.Persistence.Repositories;
using kiwiDeal.SharedKernel.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace kiwiDeal.Messages.Api;

public static class MessagesModule
{
    public static IServiceCollection AddMessagesModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<MessagesDbContext>(options =>
            options
                .UseNpgsql(configuration.GetConnectionString("MessagesConnection"))
                .UseSnakeCaseNamingConvention());

        services.AddScoped<IMessagesUnitOfWork, MessagesUnitOfWork>();
        services.AddScoped<IConversationRepository, ConversationRepository>();
        services.AddScoped<IMessageHubContext, MessageHubContext>();
        services.AddScoped<IOutboxMessageProvider>(sp => sp.GetRequiredService<MessagesDbContext>());
        return services;
    }
}
