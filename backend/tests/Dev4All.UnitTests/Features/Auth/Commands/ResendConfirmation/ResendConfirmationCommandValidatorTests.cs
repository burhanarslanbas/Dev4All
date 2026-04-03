using Dev4All.Application.Features.Auth.Commands.ResendConfirmation;

namespace Dev4All.UnitTests.Features.Auth.Commands.ResendConfirmation;

public class ResendConfirmationCommandValidatorTests
{
    [Fact]
    public void Validate_WhenEmailIsEmpty_ShouldReturnValidationError()
    {
        var validator = new ResendConfirmationCommandValidator();
        var command = new ResendConfirmationCommand(string.Empty);

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(ResendConfirmationCommand.Email));
    }

    [Fact]
    public void Validate_WhenEmailIsValid_ShouldBeValid()
    {
        var validator = new ResendConfirmationCommandValidator();
        var command = new ResendConfirmationCommand("user@example.com");

        var result = validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
