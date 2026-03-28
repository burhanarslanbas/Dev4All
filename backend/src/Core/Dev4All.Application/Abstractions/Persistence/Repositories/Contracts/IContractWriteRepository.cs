using Dev4All.Application.Abstractions.Persistence.Repositories.Base;
using Dev4All.Domain.Entities;

namespace Dev4All.Application.Abstractions.Persistence.Repositories.Contracts;

/// <summary>Write operations for <see cref="Contract"/> persistence.</summary>
public interface IContractWriteRepository : IWriteRepository<Contract>;
