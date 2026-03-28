using Dev4All.Application.Abstractions.Persistence.Repositories.ContractRevisions;
using Dev4All.Domain.Entities;
using Dev4All.Persistence.Context;

namespace Dev4All.Persistence.Repositories.ContractRevisions;

/// <summary>EF Core implementation of <see cref="IContractRevisionWriteRepository"/>.</summary>
public sealed class ContractRevisionWriteRepository(Dev4AllDbContext context) : IContractRevisionWriteRepository
{
    public async Task AddAsync(ContractRevision entity, CancellationToken cancellationToken = default)
        => await context.ContractRevisions.AddAsync(entity, cancellationToken);

    public async Task AddRangeAsync(IEnumerable<ContractRevision> entities, CancellationToken cancellationToken = default)
        => await context.ContractRevisions.AddRangeAsync(entities, cancellationToken);

    public void Update(ContractRevision entity)
        => context.ContractRevisions.Update(entity);

    public void UpdateRange(IEnumerable<ContractRevision> entities)
        => context.ContractRevisions.UpdateRange(entities);

    public void Remove(ContractRevision entity)
        => context.ContractRevisions.Remove(entity);

    public void RemoveRange(IEnumerable<ContractRevision> entities)
        => context.ContractRevisions.RemoveRange(entities);
}
