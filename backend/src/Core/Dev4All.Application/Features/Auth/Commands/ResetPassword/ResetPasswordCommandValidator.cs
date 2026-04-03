using FluentValidation;

namespace Dev4All.Application.Features.Auth.Commands.ResetPassword;

public sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta boş olamaz.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token boş olamaz.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Şifre boş olamaz.")
            .MinimumLength(8).WithMessage("Şifre en az 8 karakter olmalıdır.")
            .Matches("[A-Z]").WithMessage("Şifre en az 1 büyük harf içermelidir.")
            .Matches("[a-z]").WithMessage("Şifre en az 1 küçük harf içermelidir.")
            .Matches("[0-9]").WithMessage("Şifre en az 1 rakam içermelidir.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Şifre en az 1 özel karakter içermelidir.");
    }
}
