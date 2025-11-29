using Application.Repositories.Abstractions;
using Application.Repositories.Abstractions.Repositories;
using Infrastructure.Repositories.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Repositories;

public static class InfrastructureRepositoriesRegistrar
{
    public static IServiceCollection AddRepositoriesInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<INotificationRepository, NotificationRepository>();

        return services;
    }
}
