using Application.Abstractions.Factories;
using Application.Contracts.Commands;
using Application.Contracts.Responses;
using Application.Repositories.Abstractions;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Implementation.Handlers;

public class SendNotificationCommandHandler 
    : IRequestHandler<SendNotificationCommand, List<SendNotificationResponse>>
{
    private readonly INotificationStrategyFactory _factory;
    private readonly IUnitOfWork _unitOfWork;

    public SendNotificationCommandHandler(
        IUnitOfWork unitOfWork,
        INotificationStrategyFactory factory)
    {
        _unitOfWork = unitOfWork;
        _factory = factory;
    }

    public async Task<List<SendNotificationResponse>> Handle(
        SendNotificationCommand command,
        CancellationToken cancellationToken)
    {
        var notifications = command.Recipients.Select(recipient =>
            new Notification
            (
                command.Type,
                command.Title,
                command.Content,
                recipient
            )).ToArray();

        try 
        {
            // Сохраняем в БД как Pending
            await _unitOfWork.NotificationRepository.AddRangeAsync(notifications, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InternalServerException("Ошибка сохранения уведомления.", ex.Message);
        }

        //Осуществляем отправку уведомления
        var strategy = _factory.Create(command.Type);

        var responces = new List<SendNotificationResponse>();
        foreach (var notification in notifications) 
        {
            var success = await strategy.SendAsync(
                notification.Title, notification.Content, notification.Recipient, cancellationToken);

            // Обновляем статус уведомления
            if (success)
                notification.SetSuccess();
            else
                notification.SetFailed();
            _unitOfWork.NotificationRepository.Update(notification);

            responces.Add(new SendNotificationResponse(
                notification.Id,
                success,
                success ? "Sent" : "Failed"));
        }


        try 
        {
            //Сохранем в базу с обновленным статусом
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex) 
        {
            throw new InternalServerException(ex.Message);
        }


        return responces;
    }
}
