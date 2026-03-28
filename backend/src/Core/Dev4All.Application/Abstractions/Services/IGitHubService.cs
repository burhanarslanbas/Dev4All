using Dev4All.Domain.Entities;

namespace Dev4All.Application.Abstractions.Services;

/// <summary>Abstraction for GitHub webhook processing and signature validation.</summary>
public interface IGitHubService
{
    /// <summary>Validates a GitHub Webhook HMAC-SHA256 signature against the payload.</summary>
    bool ValidateWebhookSignature(string payload, string signature, string secret);

    /// <summary>Parses a push event JSON payload into a list of <see cref="GitHubLog"/> entries.</summary>
    List<GitHubLog> ParsePushEvent(string jsonPayload, Guid projectId);
}
