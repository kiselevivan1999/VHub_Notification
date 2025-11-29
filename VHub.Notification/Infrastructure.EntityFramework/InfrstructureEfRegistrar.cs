using Domain.Exceptions;
using Infrastructure.EntityFramework.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace Infrastructure.EntityFramework;

public static class InfrstructureEfRegistrar
{
    public static IServiceCollection AddEntityFrameworkInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        try
        {
            var dbConnectionString = GetDbConnectionString(configuration);
            services.AddDbContext<NotificationDbContext>(conf =>
                conf.UseNpgsql(dbConnectionString!));
        }
        catch
        {
            throw new InternalServerException("Ошибка инициализации контекста БД.");
        }
        return services;
    }

    private static string GetDbConnectionString(IConfiguration configuration)
    {
        string sectionName = "AppDataBase";
        var section = configuration.GetSection(sectionName);

        var dbHost = section["Host"];
        var dbDatabase = section["Database"];
        var dbUsername = section["User"];
        var dbPassword = section["Password"];
        var dbPort = section["Port"];

        var connectionStringBuilder = new StringBuilder();
        connectionStringBuilder.Append($"Host={dbHost};");
        connectionStringBuilder.Append($"Port={dbPort};");
        connectionStringBuilder.Append($"Database={dbDatabase};");
        connectionStringBuilder.Append($"User Id={dbUsername};");
        connectionStringBuilder.Append($"Password={dbPassword};");

        return connectionStringBuilder.ToString();
    }

    public async static Task MigrateDatabase(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();

        if (context == null)
            throw new InternalServerException("Контекст базы данных не инициализирован");

        //Накатываем миграции
        await context.Database.MigrateAsync();
    }
}
