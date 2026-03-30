using Dev4All.Domain.Enums;
using MediatR;

namespace Dev4All.Application.Features.Auth.Commands.RegisterUser;

/// <summary>Creates a new user account with the specified role.</summary>
public sealed record RegisterUserCommand(
    string Name,
    string Email,
    string Password,
    UserRole Role
) : IRequest<RegisterUserResponse>;
