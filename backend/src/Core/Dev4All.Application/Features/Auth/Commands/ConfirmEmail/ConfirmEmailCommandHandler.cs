using Dev4All.Application.Abstractions.Auth;
using MediatR;

namespace Dev4All.Application.Features.Auth.Commands.ConfirmEmail;

public sealed class ConfirmEmailCommandHandler(
    IIdentityService identityService) : IRequestHandler<ConfirmEmailCommand, ConfirmEmailResponse>
{
    public async Task<ConfirmEmailResponse> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var (succeeded, _) = await identityService.ConfirmEmailAsync(
            request.UserId,
            request.Token,
            cancellationToken);

        return succeeded
            ? new ConfirmEmailResponse(true, "E-posta doğrulandı.")
            : new ConfirmEmailResponse(false, "E-posta doğrulama işlemi başarısız.");
    }
}
