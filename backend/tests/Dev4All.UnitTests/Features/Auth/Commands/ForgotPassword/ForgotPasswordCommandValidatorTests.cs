using Dev4All.Application.Features.Auth.Commands.ForgotPassword;
using FluentValidation.TestHelper;

namespace Dev4All.UnitTests.Features.Auth.Commands.ForgotPassword;

public sealed class ForgotPasswordCommandValidatorTests
{
    private readonly ForgotPasswordCommandValidator _validator = new();

    [Fact]
    public void ValidateWithValidEmailShouldNotHaveValidationError()
    {
        var command = new ForgotPasswordCommand("user@example.com");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void ValidateWithEmptyEmailShouldHaveValidationError()
    {
        var command = new ForgotPasswordCommand(string.Empty);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void ValidateWithTooLongEmailShouldHaveValidationError()
    {
        var localPart = new string('a', 252);
        var command = new ForgotPasswordCommand($"{localPart}@x.com");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void ValidateWithInvalidEmailFormatShouldHaveValidationError()
    {
        var command = new ForgotPasswordCommand("not-an-email");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }
}
