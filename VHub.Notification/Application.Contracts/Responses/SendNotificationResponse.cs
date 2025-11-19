namespace Application.Contracts.Responses;

public class SendNotificationResponse()
{
    /// <summary>
    /// Идентификатор отправленного уведомлений
    /// </summary>
    public required string NotificationId { get; set; }
    /// <summary>
    /// Статус: true - доставлено, false - не доставлено
    /// </summary>
    public bool Success { get; set; }
    /// <summary>
    /// Сообщение (опционально)
    /// </summary>
    public string? Message { get; set; }

    public SendNotificationResponse(string NotificationId, 
        bool Success, string Message)
    { }
}
