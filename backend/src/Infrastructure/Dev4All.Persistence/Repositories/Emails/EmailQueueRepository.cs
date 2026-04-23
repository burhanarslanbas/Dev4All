using Dev4All.Application.Abstractions.Persistence.Repositories.Emails;
using Dev4All.Domain.Entities;
using Dev4All.Domain.Enums;
using Dev4All.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Dev4All.Persistence.Repositories.Emails;

/// <summary>EF Core implementation of <see cref="IEmailQueueRepository"/>.</summary>
public sealed class EmailQueueRepository(Dev4AllDbContext context) : IEmailQueueRepository
{
    public async Task AddAsync(EmailQueue email, CancellationToken cancellationToken = default)
        => await context.EmailQueue.AddAsync(email, cancellationToken);

    public async Task<IReadOnlyList<EmailQueue>> GetPendingBatchAsync(int batchSize, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await context.EmailQueue
            .Where(e => e.Status == EmailStatus.Pending
                        && (e.NextAttemptAt == null || e.NextAttemptAt <= now))
            .OrderBy(e => e.CreatedDate)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public void Update(EmailQueue email) => context.EmailQueue.Update(email);
}
