namespace Dev4All.Application.Options;

/// <summary>Configuration for the background email dispatch job.</summary>
public sealed class EmailDispatchOptions
{
    public const string SectionName = "EmailDispatch";

    /// <summary>Cron expression controlling how often the job runs. Defaults to once per minute.</summary>
    public string CronExpression { get; set; } = "0 0/1 * * * ?";

    /// <summary>Maximum rows claimed per job execution.</summary>
    public int BatchSize { get; set; } = 20;

    /// <summary>Maximum attempts per row before it is marked <c>Failed</c>.</summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>Base back-off delay in seconds (multiplied by retry count for linear back-off).</summary>
    public int RetryBackoffSeconds { get; set; } = 60;

    /// <summary>Master switch to disable the job in environments where SMTP is unavailable.</summary>
    public bool Enabled { get; set; } = true;
}
