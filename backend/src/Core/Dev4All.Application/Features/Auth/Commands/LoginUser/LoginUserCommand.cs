using MediatR;

namespace Dev4All.Application.Features.Auth.Commands.LoginUser;

/// <summary>Authenticates a user and returns a signed JWT token.</summary>
public sealed record LoginUserCommand(
    string Email,
    string Password
) : IRequest<LoginUserResponse>;
