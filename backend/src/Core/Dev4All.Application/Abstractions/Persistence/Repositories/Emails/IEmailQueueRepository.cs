using Dev4All.Domain.Entities;

namespace Dev4All.Application.Abstractions.Persistence.Repositories.Emails;

/// <summary>
/// Persistence operations for the outbound email queue.
/// Enqueues new messages and supplies ready-to-send rows to the dispatch job.
/// </summary>
public interface IEmailQueueRepository
{
    Task AddAsync(EmailQueue email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns rows whose status is <c>Pending</c> and whose <c>NextAttemptAt</c>
    /// is null or &lt;= now, ordered by oldest-first, up to <paramref name="batchSize"/> items.
    /// </summary>
    Task<IReadOnlyList<EmailQueue>> GetPendingBatchAsync(int batchSize, CancellationToken cancellationToken = default);

    void Update(EmailQueue email);
}
