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
    public Dictionary<string, object> Metadata { get; set; } = new();

    public SendNotificationCommand(string type, string title, string content, string recipient, string subject = null)
    {
        Type = type;
        Title = title;
        Content = content;
        Recipient = recipient;
        Subject = subject;
    }
}