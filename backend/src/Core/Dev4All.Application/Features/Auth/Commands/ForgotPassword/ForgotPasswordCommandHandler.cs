using Dev4All.Application.Abstractions.Auth;
using Dev4All.Application.Abstractions.Services;
using Dev4All.Application.Options;
using MediatR;
using Microsoft.Extensions.Options;

namespace Dev4All.Application.Features.Auth.Commands.ForgotPassword;

public sealed class ForgotPasswordCommandHandler(
    IIdentityService identityService,
    IEmailNotificationService emailNotificationService,
    IOptions<AuthOptions> authOptions) : IRequestHandler<ForgotPasswordCommand, ForgotPasswordResponse>
{
    private const string GenericMessage =
        "Eğer bu e-posta sistemde kayıtlıysa, şifre sıfırlama bağlantısı gönderilecektir.";

    public async Task<ForgotPasswordResponse> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var resetToken = await identityService.GeneratePasswordResetTokenAsync(request.Email, cancellationToken);

        if (!string.IsNullOrWhiteSpace(resetToken))
        {
            var resetUrl = BuildResetUrl(request.Email, resetToken, authOptions.Value.PasswordResetUrlTemplate);
            await emailNotificationService.QueuePasswordResetEmailAsync(request.Email, resetUrl, cancellationToken);
        }

        return new ForgotPasswordResponse(GenericMessage);
    }

    private static string BuildResetUrl(string email, string resetToken, string template)
    {
        return template
            .Replace("{email}", Uri.EscapeDataString(email), StringComparison.Ordinal)
            .Replace("{token}", Uri.EscapeDataString(resetToken), StringComparison.Ordinal);
    }
}
