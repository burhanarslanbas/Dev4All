using Dev4All.Application.Abstractions.Persistence.Repositories.Bids;
using Dev4All.Domain.Entities;
using Dev4All.Persistence.Context;

namespace Dev4All.Persistence.Repositories.Bids;

/// <summary>EF Core implementation of <see cref="IBidWriteRepository"/>.</summary>
public sealed class BidWriteRepository(Dev4AllDbContext context) : IBidWriteRepository
{
    public async Task AddAsync(Bid entity, CancellationToken cancellationToken = default)
        => await context.Bids.AddAsync(entity, cancellationToken);

    public async Task AddRangeAsync(IEnumerable<Bid> entities, CancellationToken cancellationToken = default)
        => await context.Bids.AddRangeAsync(entities, cancellationToken);

    public void Update(Bid entity)
        => context.Bids.Update(entity);

    public void UpdateRange(IEnumerable<Bid> entities)
        => context.Bids.UpdateRange(entities);

    public void Remove(Bid entity)
        => context.Bids.Remove(entity);

    public void RemoveRange(IEnumerable<Bid> entities)
        => context.Bids.RemoveRange(entities);
}
