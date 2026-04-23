using Dev4All.Application.Abstractions.Services;

namespace Dev4All.IntegrationTests.Infrastructure;

/// <summary>
/// Fake email sender used in integration tests so no real SMTP traffic is generated.
/// The real outbound email path (enqueue -&gt; Quartz dispatch -&gt; SMTP) is disabled; this
/// service only exists to satisfy DI for any code paths that still request it.
/// </summary>
internal sealed class NoOpEmailService : IEmailService
{
    public Task SendAsync(string recipient, string subject, string htmlBody, CancellationToken ct = default)
        => Task.CompletedTask;
}
