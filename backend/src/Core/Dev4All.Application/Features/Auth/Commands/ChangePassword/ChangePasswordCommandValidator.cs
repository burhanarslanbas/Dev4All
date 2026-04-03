using FluentValidation;

namespace Dev4All.Application.Features.Auth.Commands.ChangePassword;

public sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Mevcut şifre boş olamaz.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Yeni şifre boş olamaz.")
            .MinimumLength(8).WithMessage("Yeni şifre en az 8 karakter olmalıdır.")
            .Matches("[A-Z]").WithMessage("Yeni şifre en az 1 büyük harf içermelidir.")
            .Matches("[0-9]").WithMessage("Yeni şifre en az 1 rakam içermelidir.");
    }
}
