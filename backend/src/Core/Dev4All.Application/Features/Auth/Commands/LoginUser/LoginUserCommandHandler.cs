using Dev4All.Application.Abstractions.Auth;
using Dev4All.Application.Options;
using Dev4All.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Options;

namespace Dev4All.Application.Features.Auth.Commands.LoginUser;

/// <summary>Handles user login and token generation.</summary>
public sealed class LoginUserCommandHandler(
    IIdentityService identityService,
    IJwtService jwtService,
    IOptions<JwtOptions> jwtOptions) : IRequestHandler<LoginUserCommand, LoginUserResponse>
{
    public async Task<LoginUserResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var (succeeded, userId, email, role) = await identityService.AuthenticateAsync(
            request.Email,
            request.Password,
            cancellationToken);

        if (!succeeded)
            throw new UnauthorizedDomainException("Geçersiz e-posta veya şifre.");

        var token = jwtService.GenerateToken(userId, email, role);
        var expiresAt = DateTime.UtcNow.AddMinutes(jwtOptions.Value.ExpiryInMinutes);
        return new LoginUserResponse(token, expiresAt, email, role);
    }
}
