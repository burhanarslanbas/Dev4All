namespace Dev4All.Application.Abstractions.Services;

/// <summary>Abstraction for sending transactional emails.</summary>
public interface IEmailService
{
    Task SendAsync(string recipient, string subject, string htmlBody, CancellationToken ct = default);
}
