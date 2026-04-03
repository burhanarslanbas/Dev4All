using Dev4All.Application.Abstractions.Auth;
using Dev4All.Application.Abstractions.Services;
using MediatR;

namespace Dev4All.Application.Features.Auth.Commands.ForgotPassword;

public sealed class ForgotPasswordCommandHandler(
    IIdentityService identityService,
    IEmailNotificationService emailNotificationService) : IRequestHandler<ForgotPasswordCommand, ForgotPasswordResponse>
{
    private const string GenericMessage =
        "Eğer bu e-posta sistemde kayıtlıysa, şifre sıfırlama bağlantısı gönderilecektir.";
    private const string ResetPasswordPath = "/reset-password";

    public async Task<ForgotPasswordResponse> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var resetToken = await identityService.GeneratePasswordResetTokenAsync(request.Email, cancellationToken);

        if (!string.IsNullOrWhiteSpace(resetToken))
        {
            var resetUrl = BuildResetUrl(request.Email, resetToken);
            await emailNotificationService.QueuePasswordResetEmailAsync(request.Email, resetUrl, cancellationToken);
        }

        return new ForgotPasswordResponse(GenericMessage);
    }

    private static string BuildResetUrl(string email, string resetToken)
    {
        var encodedEmail = Uri.EscapeDataString(email);
        var encodedToken = Uri.EscapeDataString(resetToken);
        return $"{ResetPasswordPath}?email={encodedEmail}&token={encodedToken}";
    }
}
