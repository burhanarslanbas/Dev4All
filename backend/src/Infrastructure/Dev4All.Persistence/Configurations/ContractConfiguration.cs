using Dev4All.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dev4All.Persistence.Configurations;

public class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Content).IsRequired().HasMaxLength(10000);
        builder.Property(x => x.LastRevisedById).IsRequired().HasMaxLength(450);

        builder.HasOne(x => x.Project)
            .WithOne(x => x.Contract)
            .HasForeignKey<Contract>(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Revisions)
            .WithOne(x => x.Contract)
            .HasForeignKey(x => x.ContractId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
