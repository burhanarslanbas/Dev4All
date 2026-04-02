using MediatR;

namespace Dev4All.Application.Features.Auth.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<RefreshTokenResponse>;
