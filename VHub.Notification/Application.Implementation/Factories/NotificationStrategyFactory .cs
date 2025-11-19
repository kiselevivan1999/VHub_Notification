using Application.Abstractions.Factories;
using Application.Abstractions.Strategies;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Implementation.Factories;

public class NotificationStrategyFactory : INotificationStrategyFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, Type> _strategies;

    public NotificationStrategyFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _strategies = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
    }

    public INotificationStrategy Create(string notificationType)
    {
        if (!_strategies.TryGetValue(notificationType, out var strategyType))
        {
            throw new NotSupportedException($"Notification type '{notificationType}' is not supported");
        }

        return (INotificationStrategy)_serviceProvider.GetRequiredService(strategyType);
    }

    public bool Supports(string notificationType) => _strategies.ContainsKey(notificationType);

    public void RegisterStrategy<TStrategy>(string notificationType) where TStrategy : INotificationStrategy
    {
        _strategies[notificationType] = typeof(TStrategy);
    }
}
