using Dev4All.Application.Abstractions.Auth;
using Dev4All.Persistence.Identity;
using Microsoft.AspNetCore.Identity;

namespace Dev4All.Infrastructure.Auth;

/// <summary>Implements identity operations via ASP.NET Core Identity managers.</summary>
public sealed class IdentityService(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager) : IIdentityService
{
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

    public async Task<(bool Succeeded, string UserId, string Email, string Role)> AuthenticateAsync(
        string email,
        string password,
        CancellationToken ct = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return (false, string.Empty, string.Empty, string.Empty);

        var valid = await userManager.CheckPasswordAsync(user, password);
        if (!valid)
            return (false, string.Empty, string.Empty, string.Empty);

        var roles = await userManager.GetRolesAsync(user);
        return (true, user.Id, user.Email ?? string.Empty, roles.FirstOrDefault() ?? string.Empty);
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
}
