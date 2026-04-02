using Dev4All.Application.Abstractions.Persistence.Repositories.RefreshTokens;
using Dev4All.Domain.Entities;
using Dev4All.Persistence.Context;

namespace Dev4All.Persistence.Repositories.RefreshTokens;

/// <summary>EF Core implementation of <see cref="IRefreshTokenWriteRepository"/>.</summary>
public sealed class RefreshTokenWriteRepository(Dev4AllDbContext context) : IRefreshTokenWriteRepository
{
    public async Task AddAsync(RefreshToken entity, CancellationToken cancellationToken = default)
        => await context.RefreshTokens.AddAsync(entity, cancellationToken);

    public async Task AddRangeAsync(IEnumerable<RefreshToken> entities, CancellationToken cancellationToken = default)
        => await context.RefreshTokens.AddRangeAsync(entities, cancellationToken);

    public void Update(RefreshToken entity)
        => context.RefreshTokens.Update(entity);

    public void UpdateRange(IEnumerable<RefreshToken> entities)
        => context.RefreshTokens.UpdateRange(entities);

    public void Remove(RefreshToken entity)
        => context.RefreshTokens.Remove(entity);

    public void RemoveRange(IEnumerable<RefreshToken> entities)
        => context.RefreshTokens.RemoveRange(entities);
}
