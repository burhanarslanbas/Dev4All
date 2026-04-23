using System.ComponentModel.DataAnnotations;

namespace Dev4All.Web.Models.Auth;

public sealed class RegisterViewModel
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Şifre ve şifre tekrarı eşleşmiyor.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = "Customer";

    public bool TermsAccepted { get; set; }
}
