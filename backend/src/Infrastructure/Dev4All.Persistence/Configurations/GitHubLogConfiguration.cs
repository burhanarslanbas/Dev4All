using Dev4All.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dev4All.Persistence.Configurations;

public class GitHubLogConfiguration : IEntityTypeConfiguration<GitHubLog>
{
    public void Configure(EntityTypeBuilder<GitHubLog> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.RepoUrl).IsRequired().HasMaxLength(500);
        builder.Property(x => x.Branch).IsRequired().HasMaxLength(100);
        builder.Property(x => x.CommitHash).IsRequired().HasMaxLength(40);
        builder.Property(x => x.CommitMessage).IsRequired().HasMaxLength(1000);
        builder.Property(x => x.AuthorName).IsRequired().HasMaxLength(200);
    }
}
