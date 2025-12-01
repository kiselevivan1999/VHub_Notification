using Application.Abstractions.Factories;
using Application.Abstractions.Strategies;
using Domain.Enums;
using Domain.Exceptions;

namespace Application.Implementation.Factories;

public class NotificationStrategyFactory : INotificationStrategyFactory
{
    private readonly Dictionary<NotificationTypeEnum, INotificationStrategy> _strategies;

    public NotificationStrategyFactory(IEnumerable<INotificationStrategy> strategies)
    {
        _strategies = strategies.ToDictionary(s => s.Type);
    }

    public INotificationStrategy Create(NotificationTypeEnum notificationType)
    {
        if (_strategies.TryGetValue(notificationType, out var strategy))
            return strategy;

        throw new NotAllowedException($"Уведомления типа '{notificationType}' не поддерживается");
    }

}
