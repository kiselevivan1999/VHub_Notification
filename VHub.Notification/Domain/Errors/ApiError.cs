namespace Domain.Errors;

public class ApiError
{
    /// <summary>
    /// Http код ошибки
    /// </summary>
    public int StatusCode { get; set; }
    /// <summary>
    /// Заголовок ошибки
    /// </summary>
    public string Message { get; set; }
    /// <summary>
    /// Доп информация
    /// </summary>
    public object? Details { get; set; }

    public ApiError(int statusCode, string message, object? details)
    {
        StatusCode = statusCode;
        Message = message;
        Details = details;
    }
}
