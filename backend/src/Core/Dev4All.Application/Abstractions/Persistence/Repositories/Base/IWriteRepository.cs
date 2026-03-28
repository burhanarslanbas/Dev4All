using Dev4All.Domain.Common;

namespace Dev4All.Application.Abstractions.Persistence.Repositories.Base;

/// <summary>Generic write persistence contract for <typeparamref name="TEntity"/>.</summary>
public interface IWriteRepository<TEntity> where TEntity : BaseEntity, new()
{
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    void Update(TEntity entity);

    void UpdateRange(IEnumerable<TEntity> entities);

    void Remove(TEntity entity);

    void RemoveRange(IEnumerable<TEntity> entities);
}
