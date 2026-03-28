using Dev4All.Application.Abstractions.Persistence.Repositories.Base;
using Dev4All.Domain.Entities;

namespace Dev4All.Application.Abstractions.Persistence.Repositories.Contracts;

/// <summary>Read operations for <see cref="Contract"/> persistence.</summary>
public interface IContractReadRepository : IReadRepository<Contract>
{
    Task<Contract?> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);

    Task<Contract?> GetByIdWithRevisionsAsync(Guid id, CancellationToken cancellationToken = default);
}
