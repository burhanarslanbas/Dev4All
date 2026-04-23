using Dev4All.Application.Abstractions.Auth;
using Dev4All.Persistence.Identity;
using Microsoft.AspNetCore.Identity;

namespace Dev4All.Infrastructure.Auth;

/// <summary>Implements identity operations via ASP.NET Core Identity managers.</summary>
public sealed class IdentityService(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager) : IIdentityService
{
    /// <summary>
    /// Deterministic role priority used when a user has multiple assigned roles.
    /// Higher index wins (Admin &gt; Developer &gt; Customer).
    /// </summary>
    private static readonly string[] RolePriority = ["Customer", "Developer", "Admin"];

    public async Task<(bool Succeeded, string UserId, IEnumerable<string> Errors)> CreateUserAsync(
        string name,
        string email,
        string password,
        string role,
        CancellationToken ct = default)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            Name = name
        };

        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            return (false, string.Empty, result.Errors.Select(e => e.Description));

        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));

        await userManager.AddToRoleAsync(user, role);
        return (true, user.Id, []);
    }

    public async Task<(bool Succeeded, string UserId, string Email, string Role, bool EmailConfirmed)> AuthenticateAsync(
        string email,
        string password,
        CancellationToken ct = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return (false, string.Empty, string.Empty, string.Empty, false);

        var passwordValid = await userManager.CheckPasswordAsync(user, password);
        if (!passwordValid)
            return (false, string.Empty, string.Empty, string.Empty, false);

        var roles = await userManager.GetRolesAsync(user);
        var resolvedRole = ResolveHighestPriorityRole(roles);

        return (true, user.Id, user.Email ?? string.Empty, resolvedRole, user.EmailConfirmed);
    }

    private static string ResolveHighestPriorityRole(IList<string> roles)
    {
        if (roles is null || roles.Count == 0)
            return string.Empty;

        for (var i = RolePriority.Length - 1; i >= 0; i--)
        {
            if (roles.Contains(RolePriority[i]))
                return RolePriority[i];
        }

        return roles[0];
    }

    public async Task<bool> IsInRoleAsync(string userId, string role, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId);
        return user is not null && await userManager.IsInRoleAsync(user, role);
    }

    public async Task<string?> GetUserNameAsync(string userId, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId);
        return user?.UserName;
    }

    public async Task<(string? UserId, string? Name, bool EmailConfirmed)> GetUserByEmailAsync(
        string email,
        CancellationToken ct = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        return user is null
            ? (null, null, false)
            : (user.Id, user.Name, user.EmailConfirmed);
    }

    public async Task<string?> GenerateEmailConfirmationTokenAsync(string userId, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return null;

        return await userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    public async Task<(bool Succeeded, IEnumerable<string> Errors)> ConfirmEmailAsync(
        string userId,
        string token,
        CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return (false, ["User not found."]);

        var result = await userManager.ConfirmEmailAsync(user, token);
        return (result.Succeeded, result.Errors.Select(e => e.Description));
    }

    public async Task<string?> GeneratePasswordResetTokenAsync(string email, CancellationToken ct = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return null;

        return await userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task<(bool Succeeded, IEnumerable<string> Errors)> ResetPasswordAsync(
        string email,
        string token,
        string newPassword,
        CancellationToken ct = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return (false, ["User not found."]);

        var result = await userManager.ResetPasswordAsync(user, token, newPassword);
        return (result.Succeeded, result.Errors.Select(e => e.Description));
    }

    public async Task<(bool Succeeded, IEnumerable<string> Errors)> ChangePasswordAsync(
        string userId,
        string currentPassword,
        string newPassword,
        CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return (false, ["User not found."]);

        var result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        return (result.Succeeded, result.Errors.Select(e => e.Description));
    }

    public async Task<string?> GetEmailByUserIdAsync(string userId, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId);
        return user?.Email;
    }
}
