using Dev4All.Application.Abstractions.Persistence.Repositories.GitHubLogs;
using Dev4All.Application.Common.Pagination;
using Dev4All.Domain.Entities;
using Dev4All.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Dev4All.Persistence.Repositories.GitHubLogs;

/// <summary>EF Core implementation of <see cref="IGitHubLogReadRepository"/>.</summary>
public sealed class GitHubLogReadRepository(Dev4AllDbContext context) : IGitHubLogReadRepository
{
    public async Task<IReadOnlyList<GitHubLog>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = await context.GitHubLogs.AsNoTracking().ToListAsync(cancellationToken);
        return list;
    }

    public async Task<GitHubLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.GitHubLogs.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<GitHubLog>> GetByIdsAsync(
        IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var idSet = ids.ToHashSet();
        if (idSet.Count == 0)
            return [];

        var list = await context.GitHubLogs.AsNoTracking()
            .Where(x => idSet.Contains(x.Id))
            .ToListAsync(cancellationToken);
        return list;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.GitHubLogs.AnyAsync(x => x.Id == id, cancellationToken);

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        => await context.GitHubLogs.CountAsync(cancellationToken);

    public async Task<PagedResult<GitHubLog>> ListAsync(
        int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var safePage = page < 1 ? 1 : page;
        var query = context.GitHubLogs.AsNoTracking();
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.PushedAt)
            .Skip((safePage - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return new PagedResult<GitHubLog>(items, total, safePage, pageSize);
    }

    public async Task<IReadOnlyList<GitHubLog>> GetByProjectIdAsync(
        Guid projectId, CancellationToken cancellationToken = default)
    {
        var list = await context.GitHubLogs.AsNoTracking()
            .Where(x => x.ProjectId == projectId)
            .OrderByDescending(x => x.PushedAt)
            .ToListAsync(cancellationToken);
        return list;
    }
}
