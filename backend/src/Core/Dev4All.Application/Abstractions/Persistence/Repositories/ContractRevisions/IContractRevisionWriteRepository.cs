using Dev4All.Application.Abstractions.Persistence.Repositories.Base;
using Dev4All.Domain.Entities;

namespace Dev4All.Application.Abstractions.Persistence.Repositories.ContractRevisions;

/// <summary>Write operations for <see cref="ContractRevision"/> persistence.</summary>
public interface IContractRevisionWriteRepository : IWriteRepository<ContractRevision>;
