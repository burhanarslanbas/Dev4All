using System.ComponentModel.DataAnnotations;

namespace Dev4All.Web.Models.Auth;

public sealed class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "E-posta adresi zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
    public string Email { get; set; } = string.Empty;

    public bool Submitted { get; set; }
}
