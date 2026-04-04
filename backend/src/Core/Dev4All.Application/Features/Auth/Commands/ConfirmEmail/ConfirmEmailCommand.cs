using MediatR;

namespace Dev4All.Application.Features.Auth.Commands.ConfirmEmail;

public sealed record ConfirmEmailCommand(string UserId, string Token) : IRequest<ConfirmEmailResponse>;
