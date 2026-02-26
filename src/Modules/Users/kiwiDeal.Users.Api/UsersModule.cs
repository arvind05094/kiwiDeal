using kiwiDeal.Users.Domain.Repositories;
using kiwiDeal.Users.Infrastructure;
using kiwiDeal.Users.Infrastructure.Persistence;
using kiwiDeal.Users.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace kiwiDeal.Users.Api;

public static class UsersModule
{
    public static IServiceCollection AddUsersModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<UsersDbContext>(options =>
            options
                .UseNpgsql(configuration.GetConnectionString("UsersConnection"))
                .UseSnakeCaseNamingConvention());

        services.AddScoped<IUsersUnitOfWork, UsersUnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        return services;
    }
}
