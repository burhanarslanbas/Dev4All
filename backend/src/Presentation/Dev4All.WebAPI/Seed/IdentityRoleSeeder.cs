using Dev4All.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Dev4All.WebAPI.Seed;

/// <summary>
/// Seeds baseline identity roles in an idempotent way.
/// </summary>
internal static class IdentityRoleSeeder
{
    /// <summary>
    /// Ensures required application roles exist.
    /// </summary>
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        var roles = Enum.GetNames<UserRole>();
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}
