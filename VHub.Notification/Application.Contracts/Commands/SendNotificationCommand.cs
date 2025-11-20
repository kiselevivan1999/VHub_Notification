using Application.Contracts.Requests;
using Application.Contracts.Responses;
using MediatR;

namespace Application.Contracts.Commands;

public class SendNotificationCommand : IRequest<SendNotificationResponse>
{
    public string Type { get; set; } // Email, Telegram, Push, SMS
    public string Title { get; set; }
    public string Content { get; set; }
    public string Recipient { get; set; }
    public string Subject { get; set; } // Для email

    public SendNotificationCommand(SendNotificationRequest request)
    {
        Type = request.Type;
        Title = request.Title;
        Content = request.Content;
        Recipient = request.Recipient;
        Subject = request.Subject;
    }
}