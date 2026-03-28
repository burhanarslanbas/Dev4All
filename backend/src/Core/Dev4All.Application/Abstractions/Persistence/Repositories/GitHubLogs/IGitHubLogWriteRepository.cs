using Dev4All.Application.Abstractions.Persistence.Repositories.Base;
using Dev4All.Domain.Entities;

namespace Dev4All.Application.Abstractions.Persistence.Repositories.GitHubLogs;

/// <summary>Write operations for <see cref="GitHubLog"/> persistence.</summary>
public interface IGitHubLogWriteRepository : IWriteRepository<GitHubLog>;
