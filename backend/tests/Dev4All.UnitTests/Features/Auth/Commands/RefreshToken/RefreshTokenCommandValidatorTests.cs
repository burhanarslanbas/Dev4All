using Dev4All.Application.Features.Auth.Commands.RefreshToken;

namespace Dev4All.UnitTests.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandValidatorTests
{
    [Fact]
    public void Validate_WhenTokensAreEmpty_ShouldReturnValidationErrors()
    {
        var validator = new RefreshTokenCommandValidator();
        var command = new RefreshTokenCommand(string.Empty, string.Empty);

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(RefreshTokenCommand.AccessToken));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(RefreshTokenCommand.RefreshToken));
    }

    [Fact]
    public void Validate_WhenTokensAreProvided_ShouldBeValid()
    {
        var validator = new RefreshTokenCommandValidator();
        var command = new RefreshTokenCommand("expired-access-token", "valid-refresh-token");

        var result = validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
