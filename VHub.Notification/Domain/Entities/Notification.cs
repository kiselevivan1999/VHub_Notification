using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Уведомление
/// </summary>
public class Notification : IEntity<Guid>
{
    public Guid Id { get; set;} = Guid.NewGuid();
    /// <summary>
    /// Тип уведомления. Email, tg и т.д
    /// </summary>
    private string Type { get; set; }
    /// <summary>
    /// Заголовок сообщения.
    /// </summary>
    public string Title { get; private set; }
    /// <summary>
    /// Основной текст сообщения.
    /// </summary>
    public string Content { get; private set; }
    /// <summary>
    /// Получатель сообщения (кому должно прийти)
    /// </summary>
    public string Recipient { get; private set; }
    /// <summary>
    /// Время создания уведомления
    /// </summary>
    public DateTime CreatedDate { get; } = DateTime.UtcNow;
    /// <summary>
    /// Время доставки уведомления (только для успешно доставленных).
    /// </summary>
    public DateTime? SentDate { get; private set; }
    /// <summary>
    /// Статус доставки
    /// </summary>
    public NotificationStatusEnum Status { get; set; }

    protected Notification() { }

    public Notification(NotificationTypeEnum type, string title, 
        string content, string recipient)
    {
        Type = type.ToString();
        Title = title; 
        Content = content; 
        Recipient = recipient;
        Status = NotificationStatusEnum.Pending;
    }

    /// <summary>
    /// Выдаем Type в формате enum
    /// </summary>
    public NotificationTypeEnum GetNotificationType()
        => Enum.Parse<NotificationTypeEnum>(Type);

    /// <summary>
    /// Проверяем на соответствие тип уведомелния.
    /// </summary>
    /// <param name="type">Проверяемый тип.</param>
    /// <returns>true - равны, false - не равны.</returns>
    public bool IsType(NotificationTypeEnum type)
        => GetNotificationType() == type;

    /// <summary>
    /// Помечаем уведомление, как доставленное
    /// </summary>
    public void SetSuccess() 
    {
        Status = NotificationStatusEnum.Sent;
        SentDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Помечаем уведомление, как недоставленное
    /// </summary>
    public void SetFailed()
    {
        Status = NotificationStatusEnum.Failed;
        SentDate = null;
    }
}
