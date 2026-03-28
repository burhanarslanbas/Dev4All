using Dev4All.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dev4All.Persistence.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CustomerId).IsRequired().HasMaxLength(450);
        builder.Property(x => x.AssignedDeveloperId).HasMaxLength(450);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).IsRequired().HasMaxLength(2000);
        builder.Property(x => x.Budget).HasPrecision(18, 2);
        builder.Property(x => x.Technologies).HasMaxLength(500);

        builder.HasQueryFilter(x => x.DeletedDate == null);

        builder.HasMany(x => x.Bids)
            .WithOne(x => x.Project)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.GitHubLogs)
            .WithOne(x => x.Project)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Contract)
            .WithOne(x => x.Project)
            .HasForeignKey<Contract>(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
