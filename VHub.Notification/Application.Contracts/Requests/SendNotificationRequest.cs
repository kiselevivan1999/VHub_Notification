using Domain.Enums;

namespace Application.Contracts.Requests;

public class SendNotificationRequest
{
    public NotificationTypeEnum Type { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string Recipient { get; set; }
}
