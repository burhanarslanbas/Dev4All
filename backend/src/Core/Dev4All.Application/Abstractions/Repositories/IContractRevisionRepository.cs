using Dev4All.Domain.Entities;

namespace Dev4All.Application.Abstractions.Repositories;

/// <summary>ContractRevision-specific repository operations.</summary>
public interface IContractRevisionRepository
{
    Task<List<ContractRevision>> GetByContractIdAsync(Guid contractId, CancellationToken ct = default);
    Task AddAsync(ContractRevision entity, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
