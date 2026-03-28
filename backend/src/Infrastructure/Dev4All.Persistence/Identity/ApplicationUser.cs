using Microsoft.AspNetCore.Identity;

namespace Dev4All.Persistence.Identity;

/// <summary>Application user extending ASP.NET Core Identity.</summary>
public class ApplicationUser : IdentityUser
{
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
