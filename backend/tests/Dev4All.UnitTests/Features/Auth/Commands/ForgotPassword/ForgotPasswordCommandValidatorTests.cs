using Dev4All.Application.Features.Auth.Commands.ForgotPassword;

namespace Dev4All.UnitTests.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandValidatorTests
{
    [Fact]
    public void Validate_WhenEmailIsInvalid_ShouldReturnValidationError()
    {
        var validator = new ForgotPasswordCommandValidator();
        var command = new ForgotPasswordCommand("invalid-email");

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(ForgotPasswordCommand.Email));
    }

    [Fact]
    public void Validate_WhenEmailIsValid_ShouldBeValid()
    {
        var validator = new ForgotPasswordCommandValidator();
        var command = new ForgotPasswordCommand("user@example.com");

        var result = validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
