using Dev4All.Application.Abstractions.Persistence.Repositories.Contracts;
using Dev4All.Application.Common.Pagination;
using Dev4All.Domain.Entities;
using Dev4All.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Dev4All.Persistence.Repositories.Contracts;

/// <summary>EF Core implementation of <see cref="IContractReadRepository"/>.</summary>
public sealed class ContractReadRepository(Dev4AllDbContext context) : IContractReadRepository
{
    public async Task<IReadOnlyList<Contract>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = await context.Contracts.AsNoTracking().ToListAsync(cancellationToken);
        return list;
    }

    public async Task<Contract?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Contracts.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Contract>> GetByIdsAsync(
        IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var idSet = ids.ToHashSet();
        if (idSet.Count == 0)
            return [];

        var list = await context.Contracts.AsNoTracking()
            .Where(x => idSet.Contains(x.Id))
            .ToListAsync(cancellationToken);
        return list;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Contracts.AnyAsync(x => x.Id == id, cancellationToken);

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        => await context.Contracts.CountAsync(cancellationToken);

    public async Task<PagedResult<Contract>> ListAsync(
        int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var safePage = page < 1 ? 1 : page;
        var query = context.Contracts.AsNoTracking();
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.CreatedDate)
            .Skip((safePage - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return new PagedResult<Contract>(items, total, safePage, pageSize);
    }

    public async Task<Contract?> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
        => await context.Contracts.FirstOrDefaultAsync(x => x.ProjectId == projectId, cancellationToken);

    public async Task<Contract?> GetByIdWithRevisionsAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Contracts
            .Include(x => x.Revisions)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
}
