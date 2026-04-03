using MediatR;

namespace Dev4All.Application.Features.Auth.Commands.ForgotPassword;

/// <summary>Triggers password reset token generation and queues a reset email if account exists.</summary>
public sealed record ForgotPasswordCommand(string Email) : IRequest<ForgotPasswordResponse>;
