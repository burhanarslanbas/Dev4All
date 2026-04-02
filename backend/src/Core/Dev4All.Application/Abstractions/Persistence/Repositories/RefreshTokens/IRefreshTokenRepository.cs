using Dev4All.Domain.Entities;

namespace Dev4All.Application.Abstractions.Persistence.Repositories.RefreshTokens;

/// <summary>Persistence operations for <see cref="RefreshToken"/> entities.</summary>
public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);

    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
}
