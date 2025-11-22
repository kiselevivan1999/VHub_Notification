using Application.Abstractions.Strategies;
using Domain.Enums;

namespace Application.Abstractions.Factories;

public interface INotificationStrategyFactory
{
    INotificationStrategy Create(NotificationTypeEnum notificationType);
    bool Supports(NotificationTypeEnum notificationType);
}
