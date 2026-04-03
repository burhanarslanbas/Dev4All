using Dev4All.Application.Abstractions.Persistence;
using Dev4All.Application.Abstractions.Persistence.Repositories.RefreshTokens;
using MediatR;

namespace Dev4All.Application.Features.Auth.Commands.Logout;

/// <summary>Handles user logout by revoking a persisted refresh token.</summary>
public sealed class LogoutCommandHandler(
    IRefreshTokenWriteRepository refreshTokenWriteRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<LogoutCommand, LogoutResponse>
{
    private const string SuccessMessage = "Çıkış işlemi başarılı.";

    public async Task<LogoutResponse> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = await refreshTokenWriteRepository.GetByTokenForUpdateAsync(
            request.RefreshToken,
            cancellationToken);

        if (refreshToken is null || refreshToken.IsRevoked)
        {
            return new LogoutResponse(true, SuccessMessage);
        }

        refreshToken.Revoke();
        refreshTokenWriteRepository.Update(refreshToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new LogoutResponse(true, SuccessMessage);
    }
}
