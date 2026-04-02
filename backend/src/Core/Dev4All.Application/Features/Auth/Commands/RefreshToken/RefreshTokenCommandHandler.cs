using Dev4All.Application.Abstractions.Auth;
using Dev4All.Application.Abstractions.Persistence;
using Dev4All.Application.Abstractions.Persistence.Repositories.RefreshTokens;
using Dev4All.Application.Options;
using Dev4All.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using RefreshTokenEntity = Dev4All.Domain.Entities.RefreshToken;

namespace Dev4All.Application.Features.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler(
    IJwtService jwtService,
    IIdentityService identityService,
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork,
    IOptions<JwtOptions> jwtOptions) : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(7);

    public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = jwtService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal is null)
            throw new UnauthorizedDomainException("Geçersiz access token.");

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? principal.FindFirst("sub")?.Value;
        if (string.IsNullOrWhiteSpace(userId))
            throw new UnauthorizedDomainException("Token kullanıcı bilgisi içeremiyor.");

        var tokenEntity = await refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
        if (tokenEntity is null
            || tokenEntity.UserId != userId
            || tokenEntity.IsRevoked
            || tokenEntity.ExpiresAt <= DateTime.UtcNow)
        {
            throw new UnauthorizedDomainException("Geçersiz refresh token.");
        }

        var email = await identityService.GetEmailByUserIdAsync(userId, cancellationToken);
        var role = principal.FindFirst(ClaimTypes.Role)?.Value;
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(role))
            throw new UnauthorizedDomainException("Kullanıcı bilgileri doğrulanamadı.");

        tokenEntity.Revoke();

        var newAccessToken = jwtService.GenerateToken(userId, email, role);
        var newRefreshTokenValue = jwtService.GenerateRefreshToken();
        var refreshTokenExpiresAt = DateTime.UtcNow.Add(RefreshTokenLifetime);
        var newRefreshToken = RefreshTokenEntity.Create(newRefreshTokenValue, userId, refreshTokenExpiresAt);

        await refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var accessTokenExpiresAt = DateTime.UtcNow.AddMinutes(jwtOptions.Value.ExpiryInMinutes);
        return new RefreshTokenResponse(newAccessToken, newRefreshTokenValue, accessTokenExpiresAt);
    }
}
