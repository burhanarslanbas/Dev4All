using Dev4All.Application.Abstractions.Auth;
using Dev4All.Persistence.Context;
using Dev4All.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Dev4All.Infrastructure.Auth;

/// <summary>Implements identity operations via ASP.NET Core Identity managers.</summary>
public sealed class IdentityService(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    Dev4AllDbContext dbContext,
    IPasswordHasher<ApplicationUser> passwordHasher,
    ILookupNormalizer lookupNormalizer) : IIdentityService
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

    public Task<(bool Succeeded, string UserId, string Email, string Role)> AuthenticateAsync(
        string email,
        string password,
        CancellationToken ct = default)
    {
        var normalizedEmail = lookupNormalizer.NormalizeEmail(email);
        var authData =
            (from user in dbContext.Users.AsNoTracking()
             where user.NormalizedEmail == normalizedEmail
             join userRole in dbContext.UserRoles.AsNoTracking()
                 on user.Id equals userRole.UserId into userRoleGroup
             from userRole in userRoleGroup.DefaultIfEmpty()
             join role in dbContext.Roles.AsNoTracking()
                 on userRole.RoleId equals role.Id into roleGroup
             from role in roleGroup.DefaultIfEmpty()
             select new
             {
                 user.Id,
                 user.Email,
                 user.PasswordHash,
                 Role = role != null ? role.Name : string.Empty
             })
            .FirstOrDefault();

        if (authData is null)
            return Task.FromResult((false, string.Empty, string.Empty, string.Empty));

        if (string.IsNullOrWhiteSpace(authData.PasswordHash))
            return Task.FromResult((false, string.Empty, string.Empty, string.Empty));

        var userForHash = new ApplicationUser
        {
            Id = authData.Id,
            Email = authData.Email,
            PasswordHash = authData.PasswordHash
        };

        var passwordResult = passwordHasher.VerifyHashedPassword(userForHash, authData.PasswordHash, password);
        if (passwordResult is PasswordVerificationResult.Failed)
            return Task.FromResult((false, string.Empty, string.Empty, string.Empty));

        return Task.FromResult((true, authData.Id, authData.Email ?? string.Empty, authData.Role ?? string.Empty));
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
