using Dev4All.Application.Abstractions.Persistence.Repositories.Bids;
using Dev4All.Application.Common.Pagination;
using Dev4All.Domain.Entities;
using Dev4All.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Dev4All.Persistence.Repositories.Bids;

/// <summary>EF Core implementation of <see cref="IBidReadRepository"/>.</summary>
public sealed class BidReadRepository(Dev4AllDbContext context) : IBidReadRepository
{
    public async Task<IReadOnlyList<Bid>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = await context.Bids.AsNoTracking().ToListAsync(cancellationToken);
        return list;
    }

    public async Task<Bid?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Bids.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Bid>> GetByIdsAsync(
        IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var idSet = ids.ToHashSet();
        if (idSet.Count == 0)
            return [];

        var list = await context.Bids.AsNoTracking()
            .Where(x => idSet.Contains(x.Id))
            .ToListAsync(cancellationToken);
        return list;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Bids.AnyAsync(x => x.Id == id, cancellationToken);

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        => await context.Bids.CountAsync(cancellationToken);

    public async Task<PagedResult<Bid>> ListAsync(
        int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var safePage = page < 1 ? 1 : page;
        var query = context.Bids.AsNoTracking();
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.CreatedDate)
            .Skip((safePage - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return new PagedResult<Bid>(items, total, safePage, pageSize);
    }

    public async Task<Bid?> GetByDeveloperAndProjectAsync(
        string developerId, Guid projectId, CancellationToken cancellationToken = default)
        => await context.Bids.FirstOrDefaultAsync(
            x => x.DeveloperId == developerId && x.ProjectId == projectId, cancellationToken);

    public async Task<IReadOnlyList<Bid>> GetByProjectIdAsync(
        Guid projectId, CancellationToken cancellationToken = default)
    {
        var list = await context.Bids.AsNoTracking()
            .Where(x => x.ProjectId == projectId)
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync(cancellationToken);
        return list;
    }

    public async Task<IReadOnlyList<Bid>> GetByDeveloperIdAsync(
        string developerId, CancellationToken cancellationToken = default)
    {
        var list = await context.Bids.AsNoTracking()
            .Where(x => x.DeveloperId == developerId)
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync(cancellationToken);
        return list;
    }
}
