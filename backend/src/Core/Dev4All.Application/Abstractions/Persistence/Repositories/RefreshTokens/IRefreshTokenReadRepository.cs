using Dev4All.Application.Abstractions.Persistence.Repositories.Base;
using Dev4All.Domain.Entities;

namespace Dev4All.Application.Abstractions.Persistence.Repositories.RefreshTokens;

/// <summary>Read operations for <see cref="RefreshToken"/> persistence.</summary>
public interface IRefreshTokenReadRepository : IReadRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
}
