namespace Dev4All.Infrastructure.Email;

/// <summary>
/// Well-known template keys persisted in <c>EmailQueue.TemplateKey</c>.
/// Each value maps 1:1 to a file in <c>Email/Templates/&lt;key&gt;.html</c>.
/// </summary>
public static class EmailTemplateKeys
{
    public const string VerifyEmail = "verify-email";
    public const string ResetPassword = "reset-password";
    public const string Welcome = "welcome";
    public const string PasswordChanged = "password-changed";
}
