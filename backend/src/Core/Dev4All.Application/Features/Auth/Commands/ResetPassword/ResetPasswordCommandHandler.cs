using Dev4All.Application.Abstractions.Auth;
using MediatR;

namespace Dev4All.Application.Features.Auth.Commands.ResetPassword;

public sealed class ResetPasswordCommandHandler(
    IIdentityService identityService) : IRequestHandler<ResetPasswordCommand, ResetPasswordResponse>
{
    public async Task<ResetPasswordResponse> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var (succeeded, errors) = await identityService.ResetPasswordAsync(
            request.Email,
            request.Token,
            request.NewPassword,
            cancellationToken);

        if (succeeded)
            return new ResetPasswordResponse(true, "Şifreniz başarıyla sıfırlandı.");

        var errorMessage = errors.Any() ? string.Join(", ", errors) : "Şifre sıfırlama işlemi başarısız.";
        return new ResetPasswordResponse(false, errorMessage);
    }
}
