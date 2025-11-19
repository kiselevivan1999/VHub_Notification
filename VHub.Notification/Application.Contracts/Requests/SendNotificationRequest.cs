namespace Application.Contracts.Requests;

public class SendNotificationRequest
{
    public string Type { get; set; } = "Email";
    public string Title { get; set; }
    public string Content { get; set; }
    public string Recipient { get; set; }
    public string Subject { get; set; }
}
