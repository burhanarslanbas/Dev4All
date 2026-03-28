using Dev4All.Domain.Entities;

namespace Dev4All.Application.Abstractions.Repositories;

/// <summary>GitHubLog-specific repository operations.</summary>
public interface IGitHubLogRepository
{
    Task<List<GitHubLog>> GetByProjectIdAsync(Guid projectId, CancellationToken ct = default);
    Task AddAsync(GitHubLog entity, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<GitHubLog> entities, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
