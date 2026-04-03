using Dev4All.Application.Abstractions.Auth;
using Dev4All.Application.Abstractions.Services;
using Dev4All.Application.Options;
using MediatR;
using Microsoft.Extensions.Options;
using System.Net;

namespace Dev4All.Application.Features.Auth.Commands.ForgotPassword;

/// <summary>Handles forgot password requests without revealing account existence.</summary>
public sealed class ForgotPasswordCommandHandler(
    IIdentityService identityService,
    IEmailNotificationService emailNotificationService,
    IOptions<FrontendOptions> frontendOptions) : IRequestHandler<ForgotPasswordCommand, ForgotPasswordResponse>
{
    private const string GenericMessage = "Eğer bu e-posta adresi sistemde kayıtlıysa, şifre sıfırlama bağlantısı gönderilecektir.";

    public async Task<ForgotPasswordResponse> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var (userExists, userName, resetToken) = await identityService.GeneratePasswordResetTokenAsync(
            request.Email,
            cancellationToken);

        if (userExists)
        {
            var redirectUrl = BuildRedirectUrl(frontendOptions.Value.PasswordResetUrl, request.Email, resetToken);
            await emailNotificationService.QueuePasswordResetEmailAsync(
                request.Email,
                userName,
                resetToken,
                redirectUrl,
                cancellationToken);
        }

        return new ForgotPasswordResponse(GenericMessage);
    }

    private static string BuildRedirectUrl(string passwordResetUrl, string email, string resetToken)
    {
        var separator = passwordResetUrl.Contains('?', StringComparison.Ordinal) ? "&" : "?";
        return $"{passwordResetUrl}{separator}email={WebUtility.UrlEncode(email)}&token={WebUtility.UrlEncode(resetToken)}";
    }
}
