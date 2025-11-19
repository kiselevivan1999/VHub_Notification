using Application.Abstractions.Factories;
using Application.Abstractions.Strategies;
using Infrastructure.Email.Settings;
using Infrastructure.Email.Strategies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Email;

public static class Registrar
{
    public static IServiceCollection AddEmailInfrastructure(this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Настройки
        services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));

        // Стратегия
        services.AddScoped<EmailNotificationStrategy>();
        services.AddScoped<IEmailNotificationStrategy>(sp =>
            sp.GetRequiredService<EmailNotificationStrategy>());

        return services;
    }
    public static void RegisterEmailStrategy(this INotificationStrategyFactory factory)
    {
        factory.RegisterStrategy<EmailNotificationStrategy>("Email");
    }
}
