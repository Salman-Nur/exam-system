using ExamSystem.Application.Common.Options;
using ExamSystem.Application.Common.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace ExamSystem.Infrastructure.Services;

public class HtmlEmailService : IEmailService
{
    private readonly SmtpOptions _smtpOptions;

    public HtmlEmailService(IOptions<SmtpOptions> emailSettings)
    {
        _smtpOptions = emailSettings.Value;
    }

    public async Task SendSingleEmailAsync(string receiverName, string receiverEmail, string subject,
        string body)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(_smtpOptions.SenderName, _smtpOptions.SenderEmail));

        message.To.Add(new MailboxAddress(receiverName, receiverEmail));
        message.Subject = subject;

        var builder = new BodyBuilder { HtmlBody = body };

        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();

        if (client.IsConnected is false)
        {
            await client.ConnectAsync(_smtpOptions.Host, _smtpOptions.Port,
                _smtpOptions.UseTls ? SecureSocketOptions.StartTlsWhenAvailable : SecureSocketOptions.None);
        }

        if (client.IsAuthenticated is false)
        {
            await client.AuthenticateAsync(_smtpOptions.Username, _smtpOptions.Password);
        }

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
