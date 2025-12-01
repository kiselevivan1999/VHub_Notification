using Application.Abstractions.Strategies;
using Infrastructure.Email.Settings;
using Infrastructure.Email.Strategies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Email;

public static class InfrstractureEmailRegistrar
{
    public static IServiceCollection AddEmailInfrastructure(this IServiceCollection services, 
        IConfiguration configuration)
    {
        var a = configuration.GetSection("SmtpSettings");
        // Настройки
        services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));

        // Стратегия
        services.AddScoped<EmailNotificationStrategy>();
        services.AddScoped<INotificationStrategy, EmailNotificationStrategy>();

        return services;
    }
}
