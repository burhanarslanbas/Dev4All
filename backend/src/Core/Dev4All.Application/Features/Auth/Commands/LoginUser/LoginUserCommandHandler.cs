using Dev4All.Application.Abstractions.Auth;
using Dev4All.Application.Abstractions.Persistence;
using Dev4All.Application.Abstractions.Persistence.Repositories.RefreshTokens;
using Dev4All.Application.Features.Auth.Common;
using Dev4All.Application.Options;
using Dev4All.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Options;
using RefreshTokenEntity = Dev4All.Domain.Entities.RefreshToken;

namespace Dev4All.Application.Features.Auth.Commands.LoginUser;

/// <summary>
/// Authenticates a user, issues a new JWT + refresh token pair,
/// and persists the refresh token for rotation/revocation.
/// </summary>
public sealed class LoginUserCommandHandler(
    IIdentityService identityService,
    IJwtService jwtService,
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork,
    IOptions<JwtOptions> jwtOptions,
    IOptions<AuthOptions> authOptions) : IRequestHandler<LoginUserCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var (succeeded, userId, email, role, emailConfirmed) = await identityService.AuthenticateAsync(
            request.Email,
            request.Password,
            cancellationToken);

        if (!succeeded)
            throw new AuthenticationFailedException("Geçersiz e-posta veya şifre.");

        if (authOptions.Value.RequireConfirmedEmail && !emailConfirmed)
            throw new AuthenticationFailedException("E-posta adresi doğrulanmadan giriş yapılamaz.");

        var accessToken = jwtService.GenerateToken(userId, email, role);
        var accessTokenExpiresAt = DateTime.UtcNow.AddMinutes(jwtOptions.Value.ExpiryInMinutes);

        var refreshTokenValue = jwtService.GenerateRefreshToken();
        var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(authOptions.Value.RefreshTokenLifetimeInDays);
        var refreshTokenEntity = RefreshTokenEntity.Create(refreshTokenValue, userId, refreshTokenExpiresAt);

        await refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResponse(accessToken, refreshTokenValue, accessTokenExpiresAt, email, role);
    }
}
