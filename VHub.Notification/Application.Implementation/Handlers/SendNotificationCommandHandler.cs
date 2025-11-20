using Application.Abstractions.Factories;
using Application.Contracts.Commands;
using Application.Contracts.Responses;
using Application.Repositories.Abstractions;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Implementation.Handlers;

public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, SendNotificationResponse>
{
    private readonly INotificationStrategyFactory _factory;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SendNotificationCommandHandler> _logger;

    public SendNotificationCommandHandler(
        IUnitOfWork unitOfWork,
        INotificationStrategyFactory factory,
        ILogger<SendNotificationCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _factory = factory;
        _logger = logger;
    }

    public async Task<SendNotificationResponse> Handle(
        SendNotificationCommand command,
        CancellationToken cancellationToken)
    {
        var notificationId = Guid.NewGuid();

        try
        {
            // Сохраняем в БД как Pending
            var notification = new Notification
            {
                Id = notificationId,
                Type = command.Type,
                Title = command.Title,
                Content = command.Content,
                Recipient = command.Recipient,
                Status = NotificationStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Notifications.AddAsync(notification, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Отправляем уведомление
            if (!_factory.Supports(command.Type))
                return await FailNotification(notificationId, $"Unsupported type: {command.Type}");

            var strategy = _factory.Create(command.Type);
            var success = await strategy.SendAsync(
                command.Title, command.Content, command.Recipient,
                command.Subject, command.Metadata, cancellationToken);
            // Обновляем статус в БД
            notification.Status = success ? NotificationStatus.Sent : NotificationStatus.Failed;
            notification.SentAt = success ? DateTime.UtcNow : null;

            await _unitOfWork.Notifications.UpdateAsync(notification, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SendNotificationResponse(
                notificationId,
                success,
                success ? "Sent" : "Failed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process notification {NotificationId}", notificationId);
            return new SendNotificationResponse(notificationId, false, ex.Message);
        }
    }
    private async Task<SendNotificationResponse> FailNotification(string notificationId, string error)
    {
        _logger.LogWarning("Notification failed: {Error}", error);
        return new SendNotificationResponse(notificationId, false, error);
    }
}
