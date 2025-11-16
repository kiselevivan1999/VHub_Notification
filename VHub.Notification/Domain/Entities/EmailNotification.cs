using Domain.Enums;

namespace Domain.Entities;

public class EmailNotification : Notification
{
    public EmailNotification()
    {
        Type = NotificationTypeEnum.Email;
    }
}
