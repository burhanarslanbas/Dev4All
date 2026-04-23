using Dev4All.Application.Abstractions.Persistence.Repositories.RefreshTokens;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Dev4All.Infrastructure.Auth;

[DisallowConcurrentExecution]
public sealed class RefreshTokenCleanupJob(
    IRefreshTokenWriteRepository refreshTokenWriteRepository,
    ILogger<RefreshTokenCleanupJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var cutoff = DateTime.UtcNow.AddDays(-30);
        var deleted = await refreshTokenWriteRepository.DeleteExpiredAndRevokedAsync(cutoff, context.CancellationToken);
        if (deleted > 0)
            logger.LogInformation("Deleted {Count} expired refresh tokens older than {Cutoff:yyyy-MM-dd}", deleted, cutoff);
    }
}
