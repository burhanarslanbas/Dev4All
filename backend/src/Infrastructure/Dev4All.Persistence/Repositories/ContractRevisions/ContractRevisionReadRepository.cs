using Dev4All.Application.Abstractions.Persistence.Repositories.ContractRevisions;
using Dev4All.Application.Common.Pagination;
using Dev4All.Domain.Entities;
using Dev4All.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Dev4All.Persistence.Repositories.ContractRevisions;

/// <summary>EF Core implementation of <see cref="IContractRevisionReadRepository"/>.</summary>
public sealed class ContractRevisionReadRepository(Dev4AllDbContext context) : IContractRevisionReadRepository
{
    public async Task<IReadOnlyList<ContractRevision>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = await context.ContractRevisions.AsNoTracking().ToListAsync(cancellationToken);
        return list;
    }

    public async Task<ContractRevision?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.ContractRevisions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<ContractRevision>> GetByIdsAsync(
        IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var idSet = ids.ToHashSet();
        if (idSet.Count == 0)
            return [];

        var list = await context.ContractRevisions.AsNoTracking()
            .Where(x => idSet.Contains(x.Id))
            .ToListAsync(cancellationToken);
        return list;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.ContractRevisions.AnyAsync(x => x.Id == id, cancellationToken);

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        => await context.ContractRevisions.CountAsync(cancellationToken);

    public async Task<PagedResult<ContractRevision>> ListAsync(
        int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var safePage = page < 1 ? 1 : page;
        var query = context.ContractRevisions.AsNoTracking();
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.RevisionNumber)
            .Skip((safePage - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return new PagedResult<ContractRevision>(items, total, safePage, pageSize);
    }

    public async Task<IReadOnlyList<ContractRevision>> GetByContractIdAsync(
        Guid contractId, CancellationToken cancellationToken = default)
    {
        var list = await context.ContractRevisions.AsNoTracking()
            .Where(x => x.ContractId == contractId)
            .OrderBy(x => x.RevisionNumber)
            .ToListAsync(cancellationToken);
        return list;
    }
}
