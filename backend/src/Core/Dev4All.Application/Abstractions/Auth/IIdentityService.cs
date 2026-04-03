namespace Dev4All.Application.Abstractions.Auth;

/// <summary>Abstraction for identity operations used by application handlers.</summary>
public interface IIdentityService
{
    Task<(bool Succeeded, string UserId, IEnumerable<string> Errors)> CreateUserAsync(
        string name,
        string email,
        string password,
        string role,
        CancellationToken ct = default);

    Task<(bool Succeeded, string UserId, string Email, string Role)> AuthenticateAsync(
        string email,
        string password,
        CancellationToken ct = default);

    Task<bool> IsInRoleAsync(string userId, string role, CancellationToken ct = default);

    Task<string?> GetUserNameAsync(string userId, CancellationToken ct = default);

    Task<string?> GenerateEmailConfirmationTokenAsync(string userId, CancellationToken ct = default);

    Task<(bool Succeeded, IEnumerable<string> Errors)> ConfirmEmailAsync(
        string userId,
        string token,
        CancellationToken ct = default);

    Task<string?> GeneratePasswordResetTokenAsync(string email, CancellationToken ct = default);

    Task<(bool Succeeded, IEnumerable<string> Errors)> ResetPasswordAsync(
        string email,
        string token,
        string newPassword,
        CancellationToken ct = default);

    Task<(bool Succeeded, IEnumerable<string> Errors)> ChangePasswordAsync(
        string userId,
        string currentPassword,
        string newPassword,
        CancellationToken ct = default);

    Task<string?> GetEmailByUserIdAsync(string userId, CancellationToken ct = default);

    Task<(string UserId, string Name, bool EmailConfirmed)?> GetUserInfoByEmailAsync(
        string email,
        CancellationToken ct = default);
}
