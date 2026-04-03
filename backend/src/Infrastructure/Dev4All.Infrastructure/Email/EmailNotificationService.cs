using Dev4All.Application.Abstractions.Services;

namespace Dev4All.Infrastructure.Email;

/// <summary>Queues email notifications for background processing.</summary>
public sealed class EmailNotificationService : IEmailNotificationService
{
    public Task QueuePasswordResetEmailAsync(
        string email,
        string userName,
        string resetToken,
        string redirectUrl,
        CancellationToken ct = default)
    {
        return Task.CompletedTask;
    }

    public Task QueueForgotPasswordEmailAsync(
        string email,
        string userName,
        string resetToken,
        CancellationToken ct = default)
    {
        return QueuePasswordResetEmailAsync(email, userName, resetToken, string.Empty, ct);
    }
}
