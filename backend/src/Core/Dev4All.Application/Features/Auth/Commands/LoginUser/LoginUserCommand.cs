using Dev4All.Application.Features.Auth.Common;
using MediatR;

namespace Dev4All.Application.Features.Auth.Commands.LoginUser;

/// <summary>Authenticates a user and returns access + refresh tokens.</summary>
public sealed record LoginUserCommand(
    string Email,
    string Password
) : IRequest<AuthResponse>;
