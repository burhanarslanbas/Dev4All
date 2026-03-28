using Dev4All.Domain.Common;
using Dev4All.Domain.Exceptions;

namespace Dev4All.Domain.Entities;

public class GitHubLog : BaseEntity
{
    public Guid ProjectId { get; private set; }
    public string RepoUrl { get; private set; } = string.Empty;
    public string Branch { get; private set; } = "main";
    public string CommitHash { get; private set; } = string.Empty;
    public string CommitMessage { get; private set; } = string.Empty;
    public string AuthorName { get; private set; } = string.Empty;
    public DateTime PushedAt { get; private set; }

    public Project Project { get; set; } = null!;

    public static GitHubLog Create(
        Guid projectId, string repoUrl, string branch, string commitHash, string commitMessage, string authorName, DateTime pushedAt)
    {
        if (projectId == Guid.Empty)
            throw new BusinessRuleViolationException("Project ID cannot be empty.");

        return new GitHubLog
        {
            ProjectId = projectId,
            RepoUrl = repoUrl,
            Branch = branch,
            CommitHash = commitHash,
            CommitMessage = commitMessage,
            AuthorName = authorName,
            PushedAt = pushedAt
        };
    }
}
