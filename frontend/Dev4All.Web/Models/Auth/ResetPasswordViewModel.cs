using System.ComponentModel.DataAnnotations;

namespace Dev4All.Web.Models.Auth;

public sealed class ResetPasswordViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Token { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    [Display(Name = "Yeni Şifre")]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(NewPassword))]
    [Display(Name = "Yeni Şifre (Tekrar)")]
    public string ConfirmPassword { get; set; } = string.Empty;

    public bool Completed { get; set; }
    public string? ResultMessage { get; set; }
}

public sealed record ResetPasswordApiResponse(bool Success, string Message);
