using kiwiDeal.Payments.Application;
using kiwiDeal.Payments.Domain.Repositories;
using kiwiDeal.Payments.Infrastructure;
using kiwiDeal.Payments.Infrastructure.Persistence;
using kiwiDeal.Payments.Infrastructure.Persistence.Repositories;
using kiwiDeal.SharedKernel.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace kiwiDeal.Payments.Api;

public static class PaymentsModule
{
    public static IServiceCollection AddPaymentsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<PaymentsDbContext>(options =>
            options
                .UseNpgsql(configuration.GetConnectionString("PaymentsConnection"))
                .UseSnakeCaseNamingConvention());

        services.AddScoped<IPaymentsUnitOfWork>(sp => sp.GetRequiredService<PaymentsDbContext>());
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IOutboxMessageProvider>(sp => sp.GetRequiredService<PaymentsDbContext>());
        services.AddScoped<IStripeService, StripeService>();
        services.AddScoped<IStripeWebhookVerifier, StripeWebhookVerifier>();

        services.AddOptions<StripeOptions>()
            .BindConfiguration(StripeOptions.SectionName)
            .ValidateDataAnnotations();

        return services;
    }
}
