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

    Task<(bool UserExists, string UserName, string ResetToken)> GeneratePasswordResetTokenAsync(
        string email,
        CancellationToken ct = default);
}
