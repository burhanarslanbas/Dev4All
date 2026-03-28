using System.ComponentModel.DataAnnotations;

namespace Dev4All.Application.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required(ErrorMessage = "JWT Issuer (Kimlik Sağlayıcı) değeri zorunludur.")]
    public string Issuer { get; set; } = string.Empty;

    [Required(ErrorMessage = "JWT Audience (Hedef Kitle) değeri zorunludur.")]
    public string Audience { get; set; } = string.Empty;

    [Required]
    [MinLength(32, ErrorMessage = "JWT için en az 32 karakter uzunluğunda bir SecretKey (Gizli Anahtar) vermelidir.")]
    public string SecretKey { get; set; } = string.Empty;

    public int ExpiryInMinutes { get; set; } = 60;
}
