using Dev4All.Application.Common.Pagination;
using Dev4All.Domain.Entities;

namespace Dev4All.Application.Abstractions.Repositories;

/// <summary>Project-specific repository operations.</summary>
public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Project?> GetByIdWithBidsAsync(Guid id, CancellationToken ct = default);
    Task<PagedResult<Project>> GetOpenProjectsAsync(int page, int pageSize, CancellationToken ct = default);
    Task<List<Project>> GetByCustomerIdAsync(string customerId, CancellationToken ct = default);
    Task AddAsync(Project entity, CancellationToken ct = default);
    void Update(Project entity);
    void Delete(Project entity);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
