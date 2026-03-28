using Dev4All.Application.Abstractions.Persistence.Repositories.GitHubLogs;
using Dev4All.Domain.Entities;
using Dev4All.Persistence.Context;

namespace Dev4All.Persistence.Repositories.GitHubLogs;

/// <summary>EF Core implementation of <see cref="IGitHubLogWriteRepository"/>.</summary>
public sealed class GitHubLogWriteRepository(Dev4AllDbContext context) : IGitHubLogWriteRepository
{
    public async Task AddAsync(GitHubLog entity, CancellationToken cancellationToken = default)
        => await context.GitHubLogs.AddAsync(entity, cancellationToken);

    public async Task AddRangeAsync(IEnumerable<GitHubLog> entities, CancellationToken cancellationToken = default)
        => await context.GitHubLogs.AddRangeAsync(entities, cancellationToken);

    public void Update(GitHubLog entity)
        => context.GitHubLogs.Update(entity);

    public void UpdateRange(IEnumerable<GitHubLog> entities)
        => context.GitHubLogs.UpdateRange(entities);

    public void Remove(GitHubLog entity)
        => context.GitHubLogs.Remove(entity);

    public void RemoveRange(IEnumerable<GitHubLog> entities)
        => context.GitHubLogs.RemoveRange(entities);
}
