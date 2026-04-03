using Dev4All.Application.Abstractions.Auth;
using Dev4All.Application.Abstractions.Services;
using MediatR;

namespace Dev4All.Application.Features.Auth.Commands.ResendConfirmation;

public sealed class ResendConfirmationCommandHandler(
    IIdentityService identityService,
    IEmailNotificationService emailNotificationService) : IRequestHandler<ResendConfirmationCommand, ResendConfirmationResponse>
{
    public async Task<ResendConfirmationResponse> Handle(
        ResendConfirmationCommand request,
        CancellationToken cancellationToken)
    {
        const string genericMessage = "Hesap mevcutsa doğrulama e-postası yeniden gönderildi.";

        var userInfo = await identityService.GetUserInfoByEmailAsync(request.Email, cancellationToken);
        if (userInfo is null || userInfo.Value.EmailConfirmed)
            return new ResendConfirmationResponse(genericMessage);

        var token = await identityService.GenerateEmailConfirmationTokenAsync(userInfo.Value.UserId, cancellationToken);
        if (string.IsNullOrWhiteSpace(token))
            return new ResendConfirmationResponse(genericMessage);

        await emailNotificationService.QueueConfirmationEmailAsync(
            request.Email,
            userInfo.Value.Name,
            token,
            cancellationToken);

        return new ResendConfirmationResponse(genericMessage);
    }
}
