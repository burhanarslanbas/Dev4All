using Dev4All.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dev4All.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Token).IsRequired().HasMaxLength(512);
        builder.Property(x => x.UserId).IsRequired().HasMaxLength(450);

        builder.HasIndex(x => x.Token).IsUnique();
        builder.HasIndex(x => x.UserId);
    }
}
