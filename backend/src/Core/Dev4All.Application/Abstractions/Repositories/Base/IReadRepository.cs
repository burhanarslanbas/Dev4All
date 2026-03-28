using Dev4All.Application.Common.Pagination;
using Dev4All.Domain.Common;

namespace Dev4All.Application.Abstractions.Repositories.Base;

/// <summary>Generic read-only repository contract for <typeparamref name="TEntity"/>.</summary>
public interface IReadRepository<TEntity> where TEntity : BaseEntity, new()
{
    Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<TEntity>> ListAsync(int page, int pageSize, CancellationToken cancellationToken = default);
}
