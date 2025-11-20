namespace Application.Abstractions.Strategies;

public interface INotificationStrategy
{
    string Type { get; }
    Task<bool> SendAsync(string title, string content, string recipient, string subject = null, 
        CancellationToken cancellationToken = default);
}
