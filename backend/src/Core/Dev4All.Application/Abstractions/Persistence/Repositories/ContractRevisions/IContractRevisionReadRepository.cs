using Dev4All.Application.Abstractions.Persistence.Repositories.Base;
using Dev4All.Domain.Entities;

namespace Dev4All.Application.Abstractions.Persistence.Repositories.ContractRevisions;

/// <summary>Read operations for <see cref="ContractRevision"/> persistence.</summary>
public interface IContractRevisionReadRepository : IReadRepository<ContractRevision>
{
    Task<IReadOnlyList<ContractRevision>> GetByContractIdAsync(Guid contractId, CancellationToken cancellationToken = default);
}
