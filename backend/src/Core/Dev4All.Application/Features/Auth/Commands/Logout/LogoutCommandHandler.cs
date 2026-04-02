using Dev4All.Application.Abstractions.Persistence;
using Dev4All.Application.Abstractions.Persistence.Repositories.RefreshTokens;
using MediatR;

namespace Dev4All.Application.Features.Auth.Commands.Logout;

/// <summary>Handles user logout by revoking a persisted refresh token.</summary>
public sealed class LogoutCommandHandler(
    IRefreshTokenReadRepository refreshTokenReadRepository,
    IRefreshTokenWriteRepository refreshTokenWriteRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<LogoutCommand, Unit>
{
    public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = await refreshTokenReadRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);

        if (refreshToken is not null && !refreshToken.IsRevoked)
        {
            refreshToken.Revoke();
            refreshTokenWriteRepository.Update(refreshToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Unit.Value;
    }
}
