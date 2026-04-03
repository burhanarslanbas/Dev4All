using Dev4All.Application.Abstractions.Services;

namespace Dev4All.Infrastructure.Email;

/// <summary>
/// Queues email notifications for background processing.
/// Current implementation is a placeholder until EmailQueue persistence and dispatch job are added.
/// </summary>
public sealed class EmailNotificationService : IEmailNotificationService
{
    /// <summary>
    /// Placeholder queue method for password reset notifications.
    /// TODO: Persist a queue record that will be processed by background dispatch job.
    /// </summary>
    public Task QueuePasswordResetEmailAsync(
        string email,
        string userName,
        string resetToken,
        string redirectUrl,
        CancellationToken ct = default)
    {
        return Task.CompletedTask;
    }
}
