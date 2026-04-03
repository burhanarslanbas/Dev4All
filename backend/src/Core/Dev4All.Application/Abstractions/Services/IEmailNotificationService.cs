namespace Dev4All.Application.Abstractions.Services;

/// <summary>Abstraction for queueing notification emails for background delivery.</summary>
public interface IEmailNotificationService
{
    Task QueuePasswordResetEmailAsync(string email, string userName, string resetToken, string redirectUrl, CancellationToken ct = default);

    Task QueueForgotPasswordEmailAsync(string email, string userName, string resetToken, CancellationToken ct = default);
}
