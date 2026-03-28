using Dev4All.Application.Abstractions.Services;
using Dev4All.Application.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Dev4All.Infrastructure.Email;

/// <summary>Sends transactional emails via SMTP using MailKit and <see cref="SmtpOptions"/>.</summary>
public sealed class EmailService(IOptions<SmtpOptions> options) : IEmailService
{
    private readonly SmtpOptions _smtp = options.Value;

    public async Task SendAsync(string recipient, string subject, string htmlBody, CancellationToken ct = default)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(_smtp.SenderEmail));
        message.To.Add(MailboxAddress.Parse(recipient));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = htmlBody };

        using var client = new SmtpClient();

        var secureSocket = _smtp.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None;
        await client.ConnectAsync(_smtp.Host, _smtp.Port, secureSocket, ct);

        if (!string.IsNullOrWhiteSpace(_smtp.Username))
            await client.AuthenticateAsync(_smtp.Username, _smtp.Password, ct);

        await client.SendAsync(message, ct);
        await client.DisconnectAsync(true, ct);
    }
}
