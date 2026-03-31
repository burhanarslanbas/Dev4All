using System.ComponentModel.DataAnnotations;

namespace Dev4All.Application.Options;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    [Required(ErrorMessage = "Veritabanı bağlantı dizesi (ConnectionString) zorunludur.")]
    public string ConnectionString { get; set; } = string.Empty;

    [Range(0, 10, ErrorMessage = "MaxRetryCount must be between 0 and 10.")]
    public int MaxRetryCount { get; set; } = 0;
}
