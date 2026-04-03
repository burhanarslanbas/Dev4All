using MediatR;

namespace Dev4All.Application.Features.Auth.Commands.Logout;

/// <summary>Logs out a user by revoking the provided refresh token.</summary>
public sealed record LogoutCommand(string RefreshToken) : IRequest<LogoutResponse>;
