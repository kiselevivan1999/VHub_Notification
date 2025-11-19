namespace Application.Abstractions.Strategies;

public interface INotificationStrategy
{
    string Type { get; }
    Task<bool> SendAsync(string title, string content, string recipient, string subject = null,
        Dictionary<string, object> metadata = null, CancellationToken cancellationToken = default);
}
