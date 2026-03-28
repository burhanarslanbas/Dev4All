using Dev4All.Application.Abstractions.Persistence.Repositories.Base;
using Dev4All.Domain.Entities;

namespace Dev4All.Application.Abstractions.Persistence.Repositories.Bids;

/// <summary>Read operations for <see cref="Bid"/> persistence.</summary>
public interface IBidReadRepository : IReadRepository<Bid>
{
    Task<Bid?> GetByDeveloperAndProjectAsync(
        string developerId,
        Guid projectId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Bid>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Bid>> GetByDeveloperIdAsync(string developerId, CancellationToken cancellationToken = default);
}
