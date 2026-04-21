using MediatR;

namespace Dev4All.Application.Features.Auth.Commands.ResendConfirmation;

public sealed record ResendConfirmationCommand(string Email) : IRequest<ResendConfirmationResponse>;
