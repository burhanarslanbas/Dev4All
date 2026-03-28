using Dev4All.Domain.Entities;

namespace Dev4All.Application.Abstractions.Repositories;

/// <summary>Bid-specific repository operations.</summary>
public interface IBidRepository
{
    Task<Bid?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Bid?> GetByDeveloperAndProjectAsync(string developerId, Guid projectId, CancellationToken ct = default);
    Task<List<Bid>> GetByProjectIdAsync(Guid projectId, CancellationToken ct = default);
    Task<List<Bid>> GetByDeveloperIdAsync(string developerId, CancellationToken ct = default);
    Task AddAsync(Bid entity, CancellationToken ct = default);
    void Update(Bid entity);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
