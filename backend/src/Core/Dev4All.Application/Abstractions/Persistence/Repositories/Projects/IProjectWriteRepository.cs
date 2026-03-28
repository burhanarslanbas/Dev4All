using Dev4All.Application.Abstractions.Persistence.Repositories.Base;
using Dev4All.Domain.Entities;

namespace Dev4All.Application.Abstractions.Persistence.Repositories.Projects;

/// <summary>Write operations for <see cref="Project"/> persistence.</summary>
public interface IProjectWriteRepository : IWriteRepository<Project>
{
    /// <summary>Soft-deletes the project (sets <see cref="Dev4All.Domain.Common.BaseEntity.DeletedDate"/>).</summary>
    void SoftDelete(Project entity);
}
