using Dev4All.Application.Features.Auth.Commands.ResetPassword;

namespace Dev4All.UnitTests.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommandValidatorTests
{
    [Fact]
    public void Validate_WhenCommandIsValid_ShouldBeValid()
    {
        var validator = new ResetPasswordCommandValidator();
        var command = new ResetPasswordCommand("user@example.com", "valid-token", "Valid123!");

        var result = validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WhenPasswordDoesNotMeetRequirements_ShouldReturnValidationErrors()
    {
        var validator = new ResetPasswordCommandValidator();
        var command = new ResetPasswordCommand("user@example.com", "valid-token", "short");

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(ResetPasswordCommand.NewPassword));
    }
}
