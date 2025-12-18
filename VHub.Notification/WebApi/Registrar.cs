using Application.Abstractions.Factories;
using Application.Contracts.Commands;
using Application.Contracts.Responses;
using Application.Implementation.Consumers;
using Application.Implementation.Factories;
using Application.Implementation.Handlers;
using Infrastructure.Email;
using Infrastructure.EntityFramework;
using Infrastructure.Repositories;
using KafkaFlow;
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
        services.AddKafkaServices(configuration);

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

    private static IServiceCollection AddKafkaServices(
        this IServiceCollection services, IConfiguration config)
    {
        var logger = services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
        // Получаем настройки Kafka
        var kafkaSection = config.GetSection("Kafka");
        if (kafkaSection == null)
            throw new Exception("Конфигурация для Kafka не найдена.");
        
        var brokers = kafkaSection.GetSection("Brokers").Get<string[]>();
        var groupId = kafkaSection["GroupId"];
        var topic = kafkaSection["Topic"];

        services
             .AddKafka(kafka => kafka
                 .AddCluster(cluster => cluster
                     .WithBrokers(brokers)));

        services.AddHostedService<NotifiсationConsumer>();

        return services;
    }

    public static IApplicationBuilder UseKafkaBus(this IApplicationBuilder app, IHostApplicationLifetime lifetime)
    {
        var kafkaBus = app.ApplicationServices.CreateKafkaBus();

        lifetime.ApplicationStarted.Register(async () =>
        {
            try
            {
                await kafkaBus.StartAsync(lifetime.ApplicationStopping);
                Console.WriteLine("Kafka bus started successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to start Kafka bus: {ex.Message}");
            }
        });

        lifetime.ApplicationStopping.Register(() => { kafkaBus.StopAsync().GetAwaiter().GetResult(); });

        return app;
    }
}