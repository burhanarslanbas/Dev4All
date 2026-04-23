using Dev4All.Application.Features.Auth.Common;
using MediatR;

namespace Dev4All.Application.Features.Auth.Commands.RefreshToken;

/// <summary>Exchanges an expired access token + valid refresh token for a new pair.</summary>
public sealed record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<AuthResponse>;
