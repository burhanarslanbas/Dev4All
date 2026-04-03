namespace Dev4All.Application.Abstractions.Services;

/// <summary>Abstraction for queueing transactional notification emails.</summary>
public interface IEmailNotificationService
{
    Task QueuePasswordResetEmailAsync(string email, string resetUrl, CancellationToken ct = default);
}
