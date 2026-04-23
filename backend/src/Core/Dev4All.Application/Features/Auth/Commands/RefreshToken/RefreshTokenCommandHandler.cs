using Dev4All.Application.Abstractions.Auth;
using Dev4All.Application.Abstractions.Persistence;
using Dev4All.Application.Abstractions.Persistence.Repositories.RefreshTokens;
using Dev4All.Application.Features.Auth.Common;
using Dev4All.Application.Options;
using Dev4All.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using RefreshTokenEntity = Dev4All.Domain.Entities.RefreshToken;

namespace Dev4All.Application.Features.Auth.Commands.RefreshToken;

/// <summary>
/// Exchanges an expired access token and a valid refresh token for a new pair.
/// The old refresh token is revoked (single-use rotation).
/// </summary>
public sealed class RefreshTokenCommandHandler(
    IJwtService jwtService,
    IIdentityService identityService,
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork,
    IOptions<JwtOptions> jwtOptions,
    IOptions<AuthOptions> authOptions) : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = jwtService.GetPrincipalFromExpiredToken(request.AccessToken)
            ?? throw new AuthenticationFailedException("Geçersiz access token.");

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? principal.FindFirst("sub")?.Value;
        if (string.IsNullOrWhiteSpace(userId))
            throw new AuthenticationFailedException("Token kullanıcı bilgisi içeremiyor.");

        var tokenEntity = await refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
        if (tokenEntity is null
            || tokenEntity.UserId != userId
            || tokenEntity.IsRevoked
            || tokenEntity.ExpiresAt <= DateTime.UtcNow)
        {
            throw new AuthenticationFailedException("Geçersiz refresh token.");
        }

        var email = await identityService.GetEmailByUserIdAsync(userId, cancellationToken);
        var role = principal.FindFirst(ClaimTypes.Role)?.Value;
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(role))
            throw new AuthenticationFailedException("Kullanıcı bilgileri doğrulanamadı.");

        tokenEntity.Revoke();

        var newAccessToken = jwtService.GenerateToken(userId, email, role);
        var newRefreshTokenValue = jwtService.GenerateRefreshToken();
        var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(authOptions.Value.RefreshTokenLifetimeInDays);
        var newRefreshToken = RefreshTokenEntity.Create(newRefreshTokenValue, userId, refreshTokenExpiresAt);

        await refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var accessTokenExpiresAt = DateTime.UtcNow.AddMinutes(jwtOptions.Value.ExpiryInMinutes);
        return new AuthResponse(newAccessToken, newRefreshTokenValue, accessTokenExpiresAt, email, role);
    }
}
