namespace Dev4All.Application.Abstractions.Services;

/// <summary>Abstraction for queued transactional email notifications.</summary>
public interface IEmailNotificationService
{
    Task QueueChangePasswordSuccessEmailAsync(string email, string name, CancellationToken ct = default);
}
