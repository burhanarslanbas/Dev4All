using Dev4All.Domain.Entities;

namespace Dev4All.Application.Abstractions.Repositories;

/// <summary>Contract-specific repository operations.</summary>
public interface IContractRepository
{
    Task<Contract?> GetByProjectIdAsync(Guid projectId, CancellationToken ct = default);
    Task<Contract?> GetByIdWithRevisionsAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Contract entity, CancellationToken ct = default);
    void Update(Contract entity);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
