using Application.Abstractions.Strategies;
using Domain.Enums;
using Domain.Exceptions;
using Infrastructure.Email.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Infrastructure.Email.Strategies;

public class EmailNotificationStrategy : IEmailNotificationStrategy
{
    public NotificationTypeEnum Type => NotificationTypeEnum.Email;

    private readonly SmtpSettings _settings;

    public EmailNotificationStrategy(IOptions<SmtpSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task<bool> SendAsync(
        string title,
        string content,
        string recipient,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_settings.Email))
            throw new BadRequestException("Не указан отправитель email уведомления.");
        
        var message = CreateEmailMessage(title, content, recipient);
        using var client = new SmtpClient();

        try
        {
            await client.ConnectAsync(_settings.Server, _settings.Port, GetSecureSocketOptions(), cancellationToken);
            await client.AuthenticateAsync(_settings.Email, _settings.Password, cancellationToken);

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            var errorMessage = $"Не удалось доставить email получателю '{recipient}'";
            throw new InternalServerException(errorMessage, ex.Message);
        }
    }

    private MimeMessage CreateEmailMessage(string title, string content, string recipient)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.SenderName, _settings.Email));
        message.To.Add(MailboxAddress.Parse(recipient));
        message.Subject = title;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = content,
            TextBody = content
        };

        message.Body = bodyBuilder.ToMessageBody();
        return message;
    }

    private SecureSocketOptions GetSecureSocketOptions()
    {
        return _settings.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None;
    }
}
