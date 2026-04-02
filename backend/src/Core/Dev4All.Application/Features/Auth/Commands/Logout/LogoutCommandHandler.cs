using Dev4All.Application.Abstractions.Persistence;
using Dev4All.Application.Abstractions.Persistence.Repositories.RefreshTokens;
using MediatR;

namespace Dev4All.Application.Features.Auth.Commands.Logout;

/// <summary>Handles user logout by revoking a persisted refresh token.</summary>
public sealed class LogoutCommandHandler(
    IRefreshTokenReadRepository refreshTokenReadRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<LogoutCommand, Unit>
{
    public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = await refreshTokenReadRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);

        if (refreshToken is not null && !refreshToken.IsRevoked)
        {
            refreshToken.Revoke();
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Unit.Value;
    }
}
