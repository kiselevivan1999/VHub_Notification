using Application.Abstractions.Factories;
using Application.Abstractions.Services;
using Application.Contracts.Commands;
using Application.Contracts.Responses;
using Microsoft.Extensions.Logging;

namespace Application.Implementation.Services;

public class DispatcherService : IDispatcherService
{
    private readonly INotificationStrategyFactory _factory;
    private readonly ILogger<DispatcherService> _logger;

    public NotificationDispatcher(
        INotificationStrategyFactory factory,
        ILogger<DispatcherService> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public async Task<SendNotificationResponse> DispatchAsync(
        SendNotificationCommand command,
        CancellationToken cancellationToken = default)
    {
        var notificationId = Guid.NewGuid().ToString();

        try
        {
            if (!_factory.Supports(command.Type))
            {
                return new SendNotificationResponse(notificationId, false, $"Unsupported notification type: {command.Type}");
            }
            var strategy = _factory.Create(command.Type);

            var success = await strategy.SendAsync(
                command.Title,
                command.Content,
                command.Recipient,
                command.Subject,
                command.Metadata,
                cancellationToken);

            var message = success ? "Notification sent successfully" : "Failed to send notification";

            _logger.LogInformation("Notification {NotificationId} dispatched with result: {Success}",
                notificationId, success);
            return new SendNotificationResponse(notificationId, success, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to dispatch notification {NotificationId}", notificationId);
            return new SendNotificationResponse(notificationId, false, ex.Message);
        }
    }
}
