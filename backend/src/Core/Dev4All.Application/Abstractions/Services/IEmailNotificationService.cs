namespace Dev4All.Application.Abstractions.Services;

/// <summary>
/// Abstraction for queueing transactional emails.
/// Implementations MUST persist the request to a durable outbox (e.g. EmailQueue)
/// and never perform a synchronous SMTP dispatch. Delivery is handled asynchronously
/// by the background email dispatch job.
/// </summary>
public interface IEmailNotificationService
{
    /// <summary>Queues a welcome email for a newly registered user.</summary>
    Task QueueWelcomeEmailAsync(string email, string name, CancellationToken ct = default);

    /// <summary>Queues an email-confirmation message with an opaque token for a user.</summary>
    Task QueueConfirmationEmailAsync(string userId, string email, string name, string token, CancellationToken ct = default);

    /// <summary>Queues a password-reset email containing the supplied reset URL.</summary>
    Task QueuePasswordResetEmailAsync(string email, string resetUrl, CancellationToken ct = default);

    /// <summary>Queues a notification informing the user that their password was changed.</summary>
    Task QueueChangePasswordSuccessEmailAsync(string email, string name, CancellationToken ct = default);
}
