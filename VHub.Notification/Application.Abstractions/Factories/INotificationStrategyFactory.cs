using Application.Abstractions.Strategies;

namespace Application.Abstractions.Factories;

public interface INotificationStrategyFactory
{
    INotificationStrategy Create(string notificationType);
    bool Supports(string notificationType);
}
