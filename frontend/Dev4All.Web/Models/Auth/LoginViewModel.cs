using System.ComponentModel.DataAnnotations;

namespace Dev4All.Web.Models.Auth;

public sealed class LoginViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}
