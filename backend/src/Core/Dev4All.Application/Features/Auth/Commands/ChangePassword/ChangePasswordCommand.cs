using MediatR;

namespace Dev4All.Application.Features.Auth.Commands.ChangePassword;

/// <summary>Changes password for the current authenticated user.</summary>
public sealed record ChangePasswordCommand(string CurrentPassword, string NewPassword) : IRequest<ChangePasswordResponse>;
