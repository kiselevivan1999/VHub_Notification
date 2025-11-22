using Application.Abstractions.Factories;
using Application.Abstractions.Strategies;
using Domain.Enums;
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
        services.Configure<SmtpSettings>(options => configuration.GetSection("SmtpSettings"));

        // Стратегия
        services.AddScoped<EmailNotificationStrategy>();
        services.AddScoped<INotificationStrategy, EmailNotificationStrategy>();

        return services;
    }
}
