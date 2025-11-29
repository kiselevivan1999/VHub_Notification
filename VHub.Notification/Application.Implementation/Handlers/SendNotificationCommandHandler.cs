using Application.Abstractions.Factories;
using Application.Contracts.Commands;
using Application.Contracts.Responses;
using Application.Repositories.Abstractions;
using Domain.Entities;
using Domain.Exceptions;
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
        }
        catch (Exception ex)
        {
            throw new InternalServerException("Ошибка сохранения уведомления.", ex.Message);
        }

        if (!_factory.Supports(command.Type))
            throw new NotAllowedException($"Метод не поддерживает тип {command.Type}");

        //Осуществляем отправку уведомления
        var strategy = _factory.Create(command.Type);
        var success = await strategy.SendAsync(
                command.Title, command.Content, command.Recipient,
                command.Subject, cancellationToken);

        // Обновляем статус уведомления
        if (success)
            notification.SetSuccess();
        else
            notification.SetFailed();

        try 
        {
            _unitOfWork.NotificationRepository.Update(notification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex) 
        {
            throw new InternalServerException("Ошибка обновления уведомления.", ex.Message);
        }


        return new SendNotificationResponse(
                notification.Id,
                success,
                success ? "Sent" : "Failed");

    }
}
