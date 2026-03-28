using Dev4All.Domain.Common;
using Dev4All.Domain.Entities;
using Dev4All.Persistence.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Dev4All.Persistence.Context;

/// <summary>EF Core context for Dev4All domain entities and Identity.</summary>
public class Dev4AllDbContext(DbContextOptions<Dev4AllDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Bid> Bids => Set<Bid>();
    public DbSet<GitHubLog> GitHubLogs => Set<GitHubLog>();
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<ContractRevision> ContractRevisions => Set<ContractRevision>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(Dev4AllDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedDate = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.MarkAsUpdated();
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
