using Dev4All.Application.Features.Auth.Commands.ConfirmEmail;

namespace Dev4All.UnitTests.Features.Auth.Commands.ConfirmEmail;

public class ConfirmEmailCommandValidatorTests
{
    [Fact]
    public void Validate_WhenUserIdAndTokenAreEmpty_ShouldReturnValidationErrors()
    {
        var validator = new ConfirmEmailCommandValidator();
        var command = new ConfirmEmailCommand(string.Empty, string.Empty);

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(ConfirmEmailCommand.UserId));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(ConfirmEmailCommand.Token));
    }

    [Fact]
    public void Validate_WhenUserIdAndTokenAreProvided_ShouldBeValid()
    {
        var validator = new ConfirmEmailCommandValidator();
        var command = new ConfirmEmailCommand("user-1", "token-value");

        var result = validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
