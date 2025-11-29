using Application.Abstractions.Factories;
using Application.Contracts.Commands;
using Application.Contracts.Responses;
using Application.Implementation.Factories;
using Application.Implementation.Handlers;
using Infrastructure.Email;
using Infrastructure.EntityFramework;
using Infrastructure.Repositories;
using MediatR;
using System.Reflection;

namespace WebApi;

public static class Registrar
{
    public static IServiceCollection AddServices(this IServiceCollection services,
        IConfiguration configuration)
    {

        services.AddApplicationServices();
        services.AddInfrastructureServices(configuration);

        return services;
    }

    private static IServiceCollection AddApplicationServices(this IServiceCollection services) 
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
        services.AddScoped<IRequestHandler<SendNotificationCommand, SendNotificationResponse>, 
            SendNotificationCommandHandler>();

        services.AddScoped<INotificationStrategyFactory, NotificationStrategyFactory>();
        return services;
    }

    private static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, IConfiguration config)
    {
        services.AddRepositoriesInfrastructure(config);
        services.AddEntityFrameworkInfrastructure(config);
        services.AddEmailInfrastructure(config);

        return services;
    }

}
