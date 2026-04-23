namespace Dev4All.Application.Options;

public sealed class RefreshTokenCleanupOptions
{
    public const string SectionName = "RefreshTokenCleanup";

    public string CronExpression { get; set; } = "0 0 0 * * ?";
    public int RetentionDays { get; set; } = 30;
    public bool Enabled { get; set; } = true;
}
