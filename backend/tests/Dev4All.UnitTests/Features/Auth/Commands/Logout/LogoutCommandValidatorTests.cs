using Dev4All.Application.Features.Auth.Commands.Logout;

namespace Dev4All.UnitTests.Features.Auth.Commands.Logout;

public class LogoutCommandValidatorTests
{
    [Fact]
    public void Validate_WhenRefreshTokenIsEmpty_ShouldReturnValidationError()
    {
        var validator = new LogoutCommandValidator();
        var command = new LogoutCommand(string.Empty);

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(LogoutCommand.RefreshToken));
    }

    [Fact]
    public void Validate_WhenRefreshTokenProvided_ShouldBeValid()
    {
        var validator = new LogoutCommandValidator();
        var command = new LogoutCommand("refresh-token-value");

        var result = validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
