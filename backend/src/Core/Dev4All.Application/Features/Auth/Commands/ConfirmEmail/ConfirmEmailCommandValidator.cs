using FluentValidation;

namespace Dev4All.Application.Features.Auth.Commands.ConfirmEmail;

public sealed class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Kullanıcı kimliği boş olamaz.");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Doğrulama token'ı boş olamaz.");
    }
}
