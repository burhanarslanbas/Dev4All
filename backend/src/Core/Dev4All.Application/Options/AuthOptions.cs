namespace Dev4All.Application.Options;

/// <summary>
/// Configuration options for authentication flows
/// (refresh token lifetime, email-confirmation policy, outbound email links).
/// </summary>
public sealed class AuthOptions
{
    public const string SectionName = "Auth";

    /// <summary>
    /// When true, users whose <c>EmailConfirmed</c> flag is false are rejected
    /// at login with <c>401 Unauthorized</c>. Default is false so that the MVP
    /// can register and test users without mail delivery (see FR-AUTH-07).
    /// </summary>
    public bool RequireConfirmedEmail { get; set; } = false;

    /// <summary>Refresh token sliding window lifetime, in days.</summary>
    public int RefreshTokenLifetimeInDays { get; set; } = 7;

    /// <summary>
    /// URL template used to build email-confirmation links delivered to the user.
    /// Placeholders: <c>{userId}</c>, <c>{token}</c>.
    /// </summary>
    public string EmailConfirmationUrlTemplate { get; set; } =
        "http://localhost:5019/auth/verify-email?userId={userId}&token={token}";

    /// <summary>
    /// URL template used to build password-reset links delivered to the user.
    /// Placeholders: <c>{email}</c>, <c>{token}</c>.
    /// </summary>
    public string PasswordResetUrlTemplate { get; set; } =
        "http://localhost:5019/auth/reset-password?email={email}&token={token}";
}
