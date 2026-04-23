using Dev4All.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dev4All.Persistence.Configurations;

public class EmailQueueConfiguration : IEntityTypeConfiguration<EmailQueue>
{
    public void Configure(EntityTypeBuilder<EmailQueue> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ToEmail).IsRequired().HasMaxLength(320);
        builder.Property(x => x.Subject).IsRequired().HasMaxLength(256);
        builder.Property(x => x.TemplateKey).IsRequired().HasMaxLength(128);
        builder.Property(x => x.PayloadJson).IsRequired();
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.LastError).HasMaxLength(2048);

        builder.HasIndex(x => new { x.Status, x.NextAttemptAt });
    }
}
