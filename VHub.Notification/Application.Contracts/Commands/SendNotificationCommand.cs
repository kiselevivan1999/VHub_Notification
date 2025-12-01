using Application.Contracts.Requests;
using Application.Contracts.Responses;
using Domain.Enums;
using MediatR;

namespace Application.Contracts.Commands;

public class SendNotificationCommand : IRequest<SendNotificationResponse>
{
    public NotificationTypeEnum Type { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string Recipient { get; set; }

    public SendNotificationCommand(SendNotificationRequest request)
    {
        Type = request.Type;
        Title = request.Title;
        Content = request.Content;
        Recipient = request.Recipient;
    }
}