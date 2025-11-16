using Domain.Enums;

namespace Domain.Entities;

public class Notification
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public NotificationTypeEnum Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Recipient { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SentAt { get; set; }
    public NotificationStatusEnum Status { get; set; } = NotificationStatusEnum.Pending;
}
