using Dev4All.Application.Features.Auth.Commands.ChangePassword;

namespace Dev4All.UnitTests.Features.Auth.Commands.ChangePassword;

public class ChangePasswordCommandValidatorTests
{
    [Fact]
    public void Validate_WhenPasswordsAreEmpty_ShouldReturnValidationErrors()
    {
        var validator = new ChangePasswordCommandValidator();
        var command = new ChangePasswordCommand(string.Empty, string.Empty);

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(ChangePasswordCommand.CurrentPassword));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(ChangePasswordCommand.NewPassword));
    }

    [Fact]
    public void Validate_WhenNewPasswordDoesNotMeetRules_ShouldReturnValidationErrors()
    {
        var validator = new ChangePasswordCommandValidator();
        var command = new ChangePasswordCommand("Current123", "weakpass");

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(ChangePasswordCommand.NewPassword));
    }

    [Fact]
    public void Validate_WhenRequestIsValid_ShouldBeValid()
    {
        var validator = new ChangePasswordCommandValidator();
        var command = new ChangePasswordCommand("Current123", "NewPassword1");

        var result = validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
