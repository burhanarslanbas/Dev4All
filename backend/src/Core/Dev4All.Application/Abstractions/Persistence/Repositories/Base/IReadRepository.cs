using Dev4All.Application.Common.Pagination;
using Dev4All.Domain.Common;

namespace Dev4All.Application.Abstractions.Persistence.Repositories.Base;

/// <summary>Generic read-only persistence contract for <typeparamref name="TEntity"/>.</summary>
public interface IReadRepository<TEntity> where TEntity : BaseEntity, new()
{
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TEntity>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<int> CountAsync(CancellationToken cancellationToken = default);

    Task<PagedResult<TEntity>> ListAsync(int page, int pageSize, CancellationToken cancellationToken = default);
}
