namespace Dev4All.Domain.Enums;

/// <summary>Lifecycle status of a queued transactional email.</summary>
public enum EmailStatus
{
    /// <summary>Waiting to be picked up by the dispatch job.</summary>
    Pending = 0,

    /// <summary>Currently being sent by a worker (claim marker to avoid double-send).</summary>
    Sending = 1,

    /// <summary>Delivered to the SMTP server successfully.</summary>
    Sent = 2,

    /// <summary>Exceeded the maximum retry count and will no longer be attempted.</summary>
    Failed = 3
}
