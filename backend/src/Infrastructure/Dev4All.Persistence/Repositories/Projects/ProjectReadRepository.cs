using Dev4All.Application.Abstractions.Persistence.Repositories.Projects;
using Dev4All.Application.Common.Pagination;
using Dev4All.Domain.Entities;
using Dev4All.Domain.Enums;
using Dev4All.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Dev4All.Persistence.Repositories.Projects;

/// <summary>EF Core implementation of <see cref="IProjectReadRepository"/>.</summary>
public sealed class ProjectReadRepository(Dev4AllDbContext context) : IProjectReadRepository
{
    public async Task<IReadOnlyList<Project>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = await context.Projects.AsNoTracking().ToListAsync(cancellationToken);
        return list;
    }

    public async Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Projects.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Project>> GetByIdsAsync(
        IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var idSet = ids.ToHashSet();
        if (idSet.Count == 0)
            return [];

        var list = await context.Projects
            .AsNoTracking()
            .Where(x => idSet.Contains(x.Id))
            .ToListAsync(cancellationToken);
        return list;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Projects.AnyAsync(x => x.Id == id, cancellationToken);

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        => await context.Projects.CountAsync(cancellationToken);

    public async Task<PagedResult<Project>> ListAsync(
        int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var safePage = page < 1 ? 1 : page;
        var query = context.Projects.AsNoTracking();
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.CreatedDate)
            .Skip((safePage - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return new PagedResult<Project>(items, total, safePage, pageSize);
    }

    public async Task<Project?> GetByIdWithBidsAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Projects
            .Include(x => x.Bids)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<PagedResult<Project>> GetOpenProjectsAsync(
        int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var safePage = page < 1 ? 1 : page;
        var query = context.Projects
            .AsNoTracking()
            .Where(x => x.Status == ProjectStatus.Open);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.CreatedDate)
            .Skip((safePage - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Project>(items, total, safePage, pageSize);
    }

    public async Task<IReadOnlyList<Project>> GetByCustomerIdAsync(
        string customerId, CancellationToken cancellationToken = default)
    {
        var list = await context.Projects
            .AsNoTracking()
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync(cancellationToken);
        return list;
    }
}
