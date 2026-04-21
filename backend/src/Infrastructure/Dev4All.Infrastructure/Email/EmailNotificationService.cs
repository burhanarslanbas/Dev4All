using Dev4All.Application.Abstractions.Services;

namespace Dev4All.Infrastructure.Email;

/// <summary>
/// Default implementation for <see cref="IEmailNotificationService"/> that sends transactional emails immediately.
/// </summary>
public sealed class EmailNotificationService(IEmailService emailService) : IEmailNotificationService
{
    public Task QueueChangePasswordSuccessEmailAsync(string email, string name, CancellationToken ct = default)
    {
        var safeName = string.IsNullOrWhiteSpace(name) ? "there" : name.Trim();
        var subject = "Your password was changed";
        var body = $"""
            <p>Hi {System.Net.WebUtility.HtmlEncode(safeName)},</p>
            <p>This is a confirmation that your password has been changed successfully.</p>
            <p>If you did not perform this action, please reset your password immediately and contact support.</p>
            """;

        return emailService.SendAsync(email, subject, body, ct);
    }

    public Task QueuePasswordResetEmailAsync(string email, string resetUrl, CancellationToken ct = default)
    {
        var subject = "Reset your password";
        var safeUrl = System.Net.WebUtility.HtmlEncode(resetUrl);
        var body = $"""
            <p>We received a request to reset your password.</p>
            <p><a href="{safeUrl}">Click here to reset your password</a></p>
            <p>If you did not request a password reset, you can safely ignore this email.</p>
            """;

        return emailService.SendAsync(email, subject, body, ct);
    }

    public Task QueueConfirmationEmailAsync(string email, string name, string token, CancellationToken ct = default)
    {
        var safeName = string.IsNullOrWhiteSpace(name) ? "there" : name.Trim();
        var subject = "Confirm your email";
        var body = $"""
            <p>Hi {System.Net.WebUtility.HtmlEncode(safeName)},</p>
            <p>Please confirm your email using the following token:</p>
            <p><strong>{System.Net.WebUtility.HtmlEncode(token)}</strong></p>
            <p>If you did not create an account, you can ignore this email.</p>
            """;

        return emailService.SendAsync(email, subject, body, ct);
    }
}

