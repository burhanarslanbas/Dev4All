using Dev4All.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dev4All.Persistence.Configurations;

public class BidConfiguration : IEntityTypeConfiguration<Bid>
{
    public void Configure(EntityTypeBuilder<Bid> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.DeveloperId).IsRequired().HasMaxLength(450);
        builder.Property(x => x.BidAmount).HasPrecision(18, 2);
        builder.Property(x => x.ProposalNote).IsRequired().HasMaxLength(1000);

        builder.HasQueryFilter(x => x.DeletedDate == null);
    }
}
