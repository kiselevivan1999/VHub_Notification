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
        //TODO: Для Type создать перечисление из строк
        var notification = new Notification
            (
                 command.Type,
                command.Title,
                command.Content,
                command.Recipient
            );

        try
        {
            // Сохраняем в БД как Pending
            await _unitOfWork.NotificationRepository.AddAsync(notification, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Отправляем уведомление
            if (!_factory.Supports(command.Type))
                return await FailNotification(notification.Id, $"Unsupported type: {command.Type}");

            var strategy = _factory.Create(command.Type);
            var success = await strategy.SendAsync(
                command.Title, command.Content, command.Recipient,
                command.Subject, cancellationToken);

            // Обновляем статус в БД
            if (success)
                notification.SetSuccess();
            else
                notification.SetFailed();

            _unitOfWork.NotificationRepository.Update(notification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SendNotificationResponse(
                notification.Id,
                success,
                success ? "Sent" : "Failed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process notification {NotificationId}", notification.Id);
            return new SendNotificationResponse(notification.Id, false, ex.Message);
        }
    }
    private async Task<SendNotificationResponse> FailNotification(Guid notificationId, string error)
    {
        _logger.LogWarning("Notification failed: {Error}", error);
        return new SendNotificationResponse(notificationId, false, error);
    }
}
