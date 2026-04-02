using Dev4All.Application.Abstractions.Persistence.Repositories.RefreshTokens;
using Dev4All.Domain.Entities;
using Dev4All.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Dev4All.Persistence.Repositories.RefreshTokens;

/// <summary>EF Core implementation of <see cref="IRefreshTokenRepository"/>.</summary>
public sealed class RefreshTokenRepository(Dev4AllDbContext context) : IRefreshTokenRepository
{
    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
        => await context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token, cancellationToken);

    public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
        => await context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
}
