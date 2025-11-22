using Application.Abstractions.Strategies;
using Domain.Enums;
using Infrastructure.Email.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;


namespace Infrastructure.Email.Strategies;

public class EmailNotificationStrategy : IEmailNotificationStrategy
{
    public NotificationTypeEnum Type => NotificationTypeEnum.Email;

    private readonly SmtpSettings _settings;
    private readonly ILogger<EmailNotificationStrategy> _logger;

    public EmailNotificationStrategy(
        IOptions<SmtpSettings> settings,
        ILogger<EmailNotificationStrategy> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<bool> SendAsync(
        string title,
        string content,
        string recipient,
        string subject = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var message = CreateEmailMessage(title, content, recipient, subject);

            using var client = new SmtpClient();

            await client.ConnectAsync(_settings.Server, _settings.Port, GetSecureSocketOptions(), cancellationToken);

            if (!string.IsNullOrEmpty(_settings.Username))
            {
                await client.AuthenticateAsync(_settings.Username, _settings.Password, cancellationToken);
            }

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation("Email sent successfully to {Recipient}", recipient);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Recipient}", recipient);
            return false;
        }
    }

    private MimeMessage CreateEmailMessage(string title, string content, string recipient, string subject)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        message.To.Add(MailboxAddress.Parse(recipient));
        message.Subject = subject ?? title;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = content,
            TextBody = StripHtml(content) // Автоматически создаем текстовую версию
        };

        message.Body = bodyBuilder.ToMessageBody();
        return message;
    }

    private string StripHtml(string html)
    {
        // Простая конвертация HTML в текст
        return System.Text.RegularExpressions.Regex.Replace(html, "<[^>]*>", "");
    }

    private SecureSocketOptions GetSecureSocketOptions()
    {
        return _settings.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None;
    }
}
