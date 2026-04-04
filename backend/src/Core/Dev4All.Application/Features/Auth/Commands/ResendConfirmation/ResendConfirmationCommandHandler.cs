using Dev4All.Application.Abstractions.Auth;
using Dev4All.Application.Abstractions.Services;
using MediatR;

namespace Dev4All.Application.Features.Auth.Commands.ResendConfirmation;

public sealed class ResendConfirmationCommandHandler(
    IIdentityService identityService,
    IEmailNotificationService emailNotificationService) : IRequestHandler<ResendConfirmationCommand, ResendConfirmationResponse>
{
    private const string GenericMessage = "Eğer hesap mevcutsa, e-posta doğrulama bağlantısı yeniden gönderilecektir.";

    public async Task<ResendConfirmationResponse> Handle(ResendConfirmationCommand request, CancellationToken cancellationToken)
    {
        var (userId, name, emailConfirmed) = await identityService.GetUserByEmailAsync(request.Email, cancellationToken);

        if (!string.IsNullOrWhiteSpace(userId) && !emailConfirmed)
        {
            var token = await identityService.GenerateEmailConfirmationTokenAsync(userId, cancellationToken);
            if (!string.IsNullOrWhiteSpace(token))
                await emailNotificationService.QueueConfirmationEmailAsync(request.Email, name ?? request.Email, token, cancellationToken);
        }

        return new ResendConfirmationResponse(GenericMessage);
    }
}
