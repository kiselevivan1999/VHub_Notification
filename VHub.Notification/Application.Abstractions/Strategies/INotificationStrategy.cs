using Domain.Enums;

namespace Application.Abstractions.Strategies;

public interface INotificationStrategy
{
    NotificationTypeEnum Type { get; }
    Task<bool> SendAsync(string title, string content, string recipient, 
        CancellationToken cancellationToken = default);
}
