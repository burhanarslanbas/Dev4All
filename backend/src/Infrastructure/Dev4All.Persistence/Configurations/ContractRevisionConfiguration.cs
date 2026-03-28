using Dev4All.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dev4All.Persistence.Configurations;

public class ContractRevisionConfiguration : IEntityTypeConfiguration<ContractRevision>
{
    public void Configure(EntityTypeBuilder<ContractRevision> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ContentSnapshot).IsRequired().HasMaxLength(10000);
        builder.Property(x => x.RevisedById).IsRequired().HasMaxLength(450);
        builder.Property(x => x.RevisionNote).HasMaxLength(500);
    }
}
