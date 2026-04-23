using FluentValidation;

namespace Dev4All.Application.Features.Auth.Commands.Logout;

/// <summary>Ensures a non-empty refresh token is supplied to the logout command.</summary>
public sealed class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token boş olamaz.");
    }
}
