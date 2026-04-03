using Dev4All.Application.Abstractions.Auth;
using Dev4All.Application.Abstractions.Services;
using Dev4All.Domain.Exceptions;
using MediatR;

namespace Dev4All.Application.Features.Auth.Commands.ChangePassword;

public sealed class ChangePasswordCommandHandler(
    ICurrentUser currentUser,
    IIdentityService identityService,
    IEmailNotificationService emailNotificationService) : IRequestHandler<ChangePasswordCommand, ChangePasswordResponse>
{
    public async Task<ChangePasswordResponse> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || string.IsNullOrWhiteSpace(currentUser.UserId))
            throw new UnauthorizedDomainException("Kullanıcı doğrulaması gereklidir.");

        var email = await identityService.GetEmailByUserIdAsync(currentUser.UserId, cancellationToken);
        if (string.IsNullOrWhiteSpace(email))
            throw new UnauthorizedDomainException("Kullanıcı bilgileri doğrulanamadı.");

        var (succeeded, errors) = await identityService.ChangePasswordAsync(
            currentUser.UserId,
            request.CurrentPassword,
            request.NewPassword,
            cancellationToken);

        if (!succeeded)
        {
            var message = string.Join(", ", errors);
            if (string.IsNullOrWhiteSpace(message))
                message = "Şifre değiştirilemedi.";

            return new ChangePasswordResponse(false, message);
        }

        var name = await identityService.GetUserNameAsync(currentUser.UserId, cancellationToken) ?? email;

        await emailNotificationService.QueueChangePasswordSuccessEmailAsync(email, name, cancellationToken);

        return new ChangePasswordResponse(true, "Şifre başarıyla değiştirildi.");
    }
}
