using Dev4All.Application.Abstractions.Persistence.Repositories.Base;
using Dev4All.Application.Common.Pagination;
using Dev4All.Domain.Entities;

namespace Dev4All.Application.Abstractions.Persistence.Repositories.Projects;

/// <summary>Read operations for <see cref="Project"/> persistence.</summary>
public interface IProjectReadRepository : IReadRepository<Project>
{
    Task<Project?> GetByIdWithBidsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PagedResult<Project>> GetOpenProjectsAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Project>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default);
}
