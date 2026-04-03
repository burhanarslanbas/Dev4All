namespace Dev4All.Application.Abstractions.Services;

/// <summary>Abstraction for queuing transactional email notifications.</summary>
public interface IEmailNotificationService
{
    Task QueueConfirmationEmailAsync(
        string email,
        string name,
        string token,
        CancellationToken ct = default);
}
