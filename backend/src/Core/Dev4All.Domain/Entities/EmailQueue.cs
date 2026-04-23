using Dev4All.Domain.Common;
using Dev4All.Domain.Enums;
using Dev4All.Domain.Exceptions;

namespace Dev4All.Domain.Entities;

/// <summary>
/// Durable outbox record for a transactional email.
/// Persisted by Application handlers as <see cref="EmailStatus.Pending"/>
/// and delivered asynchronously by <c>EmailDispatchJob</c>.
/// </summary>
public class EmailQueue : BaseEntity
{
    public string ToEmail { get; private set; } = string.Empty;
    public string Subject { get; private set; } = string.Empty;
    public string TemplateKey { get; private set; } = string.Empty;
    public string PayloadJson { get; private set; } = string.Empty;

    public EmailStatus Status { get; private set; } = EmailStatus.Pending;
    public int RetryCount { get; private set; }
    public DateTime? NextAttemptAt { get; private set; }
    public DateTime? SentAt { get; private set; }
    public string? LastError { get; private set; }

    public static EmailQueue Create(
        string toEmail,
        string subject,
        string templateKey,
        string payloadJson)
    {
        if (string.IsNullOrWhiteSpace(toEmail))
            throw new BusinessRuleViolationException("Email recipient cannot be empty.");
        if (string.IsNullOrWhiteSpace(subject))
            throw new BusinessRuleViolationException("Email subject cannot be empty.");
        if (string.IsNullOrWhiteSpace(templateKey))
            throw new BusinessRuleViolationException("Email template key cannot be empty.");

        return new EmailQueue
        {
            ToEmail = toEmail,
            Subject = subject,
            TemplateKey = templateKey,
            PayloadJson = payloadJson ?? "{}",
            Status = EmailStatus.Pending,
            NextAttemptAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Marks the record as in-flight. Guards against two workers sending the same row twice.
    /// </summary>
    public void MarkAsSending()
    {
        Status = EmailStatus.Sending;
        MarkAsUpdated();
    }

    public void MarkAsSent()
    {
        Status = EmailStatus.Sent;
        SentAt = DateTime.UtcNow;
        LastError = null;
        MarkAsUpdated();
    }

    /// <summary>
    /// Records a failed attempt. Requeues with <paramref name="nextAttemptAt"/> until
    /// <paramref name="maxRetries"/> is reached, after which the row is marked <see cref="EmailStatus.Failed"/>.
    /// </summary>
    public void MarkAsFailedAttempt(string error, DateTime nextAttemptAt, int maxRetries)
    {
        RetryCount += 1;
        LastError = error;

        if (RetryCount >= maxRetries)
        {
            Status = EmailStatus.Failed;
            NextAttemptAt = null;
        }
        else
        {
            Status = EmailStatus.Pending;
            NextAttemptAt = nextAttemptAt;
        }

        MarkAsUpdated();
    }
}
