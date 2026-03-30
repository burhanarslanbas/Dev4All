using FluentValidation;

namespace Dev4All.Application.Features.Auth.Commands.RegisterUser;

public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ad boş olamaz.")
            .Length(2, 100).WithMessage("Ad 2-100 karakter arası olmalıdır.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta boş olamaz.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre boş olamaz.")
            .MinimumLength(8).WithMessage("Şifre en az 8 karakter olmalıdır.")
            .Matches("[A-Z]").WithMessage("Şifre en az 1 büyük harf içermelidir.")
            .Matches("[0-9]").WithMessage("Şifre en az 1 rakam içermelidir.");

        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("Rol 'Customer' veya 'Developer' olmalıdır.");
    }
}
