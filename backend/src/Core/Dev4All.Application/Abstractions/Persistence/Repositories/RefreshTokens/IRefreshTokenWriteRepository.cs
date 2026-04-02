using Dev4All.Application.Abstractions.Persistence.Repositories.Base;
using Dev4All.Domain.Entities;

namespace Dev4All.Application.Abstractions.Persistence.Repositories.RefreshTokens;

/// <summary>Write operations for <see cref="RefreshToken"/> persistence.</summary>
public interface IRefreshTokenWriteRepository : IWriteRepository<RefreshToken>;
