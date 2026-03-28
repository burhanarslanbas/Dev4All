using Dev4All.Domain.Common;

namespace Dev4All.Application.Abstractions.Repositories.Base;

/// <summary>Generic write repository contract for <typeparamref name="TEntity"/>.</summary>
public interface IWriteRepository<TEntity> where TEntity : BaseEntity, new()
{
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void Remove(TEntity entity);
}
