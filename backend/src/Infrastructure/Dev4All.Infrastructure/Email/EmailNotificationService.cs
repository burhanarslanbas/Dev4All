using Dev4All.Application.Abstractions.Persistence;
using Dev4All.Application.Abstractions.Persistence.Repositories.Emails;
using Dev4All.Application.Abstractions.Services;
using Dev4All.Application.Options;
using Dev4All.Domain.Entities;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Dev4All.Infrastructure.Email;

/// <summary>
/// Default <see cref="IEmailNotificationService"/> implementation backed by the
/// durable <c>EmailQueue</c> outbox. All methods enqueue a pending row and
/// delegate actual delivery to the background dispatch job.
/// </summary>
public sealed class EmailNotificationService(
    IEmailQueueRepository emailQueueRepository,
    IUnitOfWork unitOfWork,
    IOptions<AuthOptions> authOptions) : IEmailNotificationService
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = null
    };

    public Task QueueWelcomeEmailAsync(string email, string name, CancellationToken ct = default)
    {
        var payload = new Dictionary<string, string>
        {
            ["Name"] = NormalizeName(name)
        };

        return EnqueueAsync(email, "Dev4All'a Hoş Geldin", EmailTemplateKeys.Welcome, payload, ct);
    }

    public Task QueueConfirmationEmailAsync(string userId, string email, string name, string token, CancellationToken ct = default)
    {
        var confirmationUrl = authOptions.Value.EmailConfirmationUrlTemplate
            .Replace("{userId}", Uri.EscapeDataString(userId), StringComparison.Ordinal)
            .Replace("{token}", Uri.EscapeDataString(token ?? string.Empty), StringComparison.Ordinal);

        var payload = new Dictionary<string, string>
        {
            ["Name"] = NormalizeName(name),
            ["ConfirmationUrl"] = confirmationUrl,
            ["Token"] = token ?? string.Empty
        };

        return EnqueueAsync(email, "E-posta Adresini Doğrula", EmailTemplateKeys.VerifyEmail, payload, ct);
    }

    public Task QueuePasswordResetEmailAsync(string email, string resetUrl, CancellationToken ct = default)
    {
        var payload = new Dictionary<string, string>
        {
            ["ResetUrl"] = resetUrl ?? string.Empty
        };

        return EnqueueAsync(email, "Şifreni Sıfırla", EmailTemplateKeys.ResetPassword, payload, ct);
    }

    public Task QueueChangePasswordSuccessEmailAsync(string email, string name, CancellationToken ct = default)
    {
        var payload = new Dictionary<string, string>
        {
            ["Name"] = NormalizeName(name)
        };

        return EnqueueAsync(email, "Şifren Başarıyla Değiştirildi", EmailTemplateKeys.PasswordChanged, payload, ct);
    }

    private async Task EnqueueAsync(
        string email,
        string subject,
        string templateKey,
        Dictionary<string, string> payload,
        CancellationToken ct)
    {
        var payloadJson = JsonSerializer.Serialize(payload, SerializerOptions);
        var entry = EmailQueue.Create(email, subject, templateKey, payloadJson);

        await emailQueueRepository.AddAsync(entry, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }

    private static string NormalizeName(string? name)
        => string.IsNullOrWhiteSpace(name) ? "değerli kullanıcımız" : name.Trim();
}
