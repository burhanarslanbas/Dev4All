using Dev4All.Application.Abstractions.Persistence.Repositories.Base;
using Dev4All.Domain.Entities;

namespace Dev4All.Application.Abstractions.Persistence.Repositories.GitHubLogs;

/// <summary>Read operations for <see cref="GitHubLog"/> persistence.</summary>
public interface IGitHubLogReadRepository : IReadRepository<GitHubLog>
{
    Task<IReadOnlyList<GitHubLog>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
}
