using MediatR;

namespace Dev4All.Application.Features.Auth.Commands.ForgotPassword;

/// <summary>Accepts an email and triggers password reset flow without account enumeration.</summary>
public sealed record ForgotPasswordCommand(string Email) : IRequest<ForgotPasswordResponse>;
