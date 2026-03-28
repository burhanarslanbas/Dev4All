using Dev4All.Application.Abstractions.Persistence.Repositories.Base;
using Dev4All.Domain.Entities;

namespace Dev4All.Application.Abstractions.Persistence.Repositories.Bids;

/// <summary>Write operations for <see cref="Bid"/> persistence.</summary>
public interface IBidWriteRepository : IWriteRepository<Bid>;
