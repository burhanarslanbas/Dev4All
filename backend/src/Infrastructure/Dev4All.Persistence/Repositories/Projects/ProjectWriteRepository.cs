using Dev4All.Application.Abstractions.Persistence.Repositories.Projects;
using Dev4All.Domain.Entities;
using Dev4All.Persistence.Context;

namespace Dev4All.Persistence.Repositories.Projects;

/// <summary>EF Core implementation of <see cref="IProjectWriteRepository"/>.</summary>
public sealed class ProjectWriteRepository(Dev4AllDbContext context) : IProjectWriteRepository
{
    public async Task AddAsync(Project entity, CancellationToken cancellationToken = default)
        => await context.Projects.AddAsync(entity, cancellationToken);

    public async Task AddRangeAsync(IEnumerable<Project> entities, CancellationToken cancellationToken = default)
        => await context.Projects.AddRangeAsync(entities, cancellationToken);

    public void Update(Project entity)
        => context.Projects.Update(entity);

    public void UpdateRange(IEnumerable<Project> entities)
        => context.Projects.UpdateRange(entities);

    public void Remove(Project entity)
        => context.Projects.Remove(entity);

    public void RemoveRange(IEnumerable<Project> entities)
        => context.Projects.RemoveRange(entities);

    public void SoftDelete(Project entity)
    {
        entity.MarkAsDeleted();
        context.Projects.Update(entity);
    }
}
