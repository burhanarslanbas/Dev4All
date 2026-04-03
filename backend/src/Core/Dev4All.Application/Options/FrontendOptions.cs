using System.ComponentModel.DataAnnotations;

namespace Dev4All.Application.Options;

public sealed class FrontendOptions
{
    public const string SectionName = "Frontend";

    [Required]
    public string PasswordResetUrl { get; set; } = string.Empty;
}
