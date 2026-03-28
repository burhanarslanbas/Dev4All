using Dev4All.Application.Abstractions.Persistence.Repositories.Contracts;
using Dev4All.Domain.Entities;
using Dev4All.Persistence.Context;

namespace Dev4All.Persistence.Repositories.Contracts;

/// <summary>EF Core implementation of <see cref="IContractWriteRepository"/>.</summary>
public sealed class ContractWriteRepository(Dev4AllDbContext context) : IContractWriteRepository
{
    public async Task AddAsync(Contract entity, CancellationToken cancellationToken = default)
        => await context.Contracts.AddAsync(entity, cancellationToken);

    public async Task AddRangeAsync(IEnumerable<Contract> entities, CancellationToken cancellationToken = default)
        => await context.Contracts.AddRangeAsync(entities, cancellationToken);

    public void Update(Contract entity)
        => context.Contracts.Update(entity);

    public void UpdateRange(IEnumerable<Contract> entities)
        => context.Contracts.UpdateRange(entities);

    public void Remove(Contract entity)
        => context.Contracts.Remove(entity);

    public void RemoveRange(IEnumerable<Contract> entities)
        => context.Contracts.RemoveRange(entities);
}
