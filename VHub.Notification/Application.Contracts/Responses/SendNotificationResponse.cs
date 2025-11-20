namespace Application.Contracts.Responses;

public class SendNotificationResponse
{
    /// <summary>
    /// Идентификатор отправленного уведомлений
    /// </summary>
    public string NotificationId { get; set;}
    /// <summary>
    /// Статус: true - доставлено, false - не доставлено
    /// </summary>
    public bool Success { get; set; }
    /// <summary>
    /// Сообщение (опционально)
    /// </summary>
    public string? Message { get; set; }

    public SendNotificationResponse(string notificationId, 
        bool success, string message)
    {
        NotificationId = notificationId;
        Success = success;
        Message = message;
    }
}
