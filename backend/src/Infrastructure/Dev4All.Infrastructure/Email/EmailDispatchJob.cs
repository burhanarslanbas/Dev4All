using Dev4All.Application.Abstractions.Persistence;
using Dev4All.Application.Abstractions.Persistence.Repositories.Emails;
using Dev4All.Application.Abstractions.Services;
using Dev4All.Application.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

namespace Dev4All.Infrastructure.Email;

/// <summary>
/// Quartz job that drains the <c>EmailQueue</c>: loads a batch of pending rows,
/// renders the HTML template and hands it to <see cref="IEmailService"/>.
/// On failure, rows are requeued with a linear back-off up to
/// <see cref="EmailDispatchOptions.MaxRetries"/> attempts.
/// </summary>
[DisallowConcurrentExecution]
public sealed class EmailDispatchJob(
    IEmailQueueRepository emailQueueRepository,
    IEmailService emailService,
    TemplateRenderer templateRenderer,
    IUnitOfWork unitOfWork,
    IOptions<EmailDispatchOptions> options,
    ILogger<EmailDispatchJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var settings = options.Value;
        if (!settings.Enabled)
            return;

        var ct = context.CancellationToken;

        var pending = await emailQueueRepository.GetPendingBatchAsync(settings.BatchSize, ct);
        if (pending.Count == 0)
            return;

        logger.LogInformation("EmailDispatchJob picked up {Count} pending email(s).", pending.Count);

        foreach (var row in pending)
        {
            try
            {
                var htmlBody = templateRenderer.Render(row.TemplateKey, row.Subject, row.PayloadJson);

                row.MarkAsSending();
                emailQueueRepository.Update(row);
                await unitOfWork.SaveChangesAsync(ct);

                await emailService.SendAsync(row.ToEmail, row.Subject, htmlBody, ct);

                row.MarkAsSent();
                emailQueueRepository.Update(row);
                await unitOfWork.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                var nextAttempt = DateTime.UtcNow.AddSeconds(settings.RetryBackoffSeconds * Math.Max(1, row.RetryCount + 1));
                row.MarkAsFailedAttempt(TrimError(ex.Message), nextAttempt, settings.MaxRetries);

                emailQueueRepository.Update(row);
                await unitOfWork.SaveChangesAsync(ct);

                logger.LogWarning(ex,
                    "Email {EmailId} delivery failed (attempt {Attempt}/{Max}). Next attempt at {NextAttempt}.",
                    row.Id, row.RetryCount, settings.MaxRetries, nextAttempt);
            }
        }
    }

    private static string TrimError(string message)
        => string.IsNullOrEmpty(message) || message.Length <= 2000 ? message ?? string.Empty : message[..2000];
}
