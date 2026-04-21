using Dev4All.Domain.Entities;

namespace Dev4All.IntegrationTests.Domain;

public class RefreshTokenTests
{
    [Fact]
    public void Create_ValidInputs_ReturnsActiveToken()
    {
        // Arrange
        var token = "refresh-token";
        var userId = "user-1";
        var expiresAt = DateTime.UtcNow.AddDays(7);

        // Act
        var refreshToken = RefreshToken.Create(token, userId, expiresAt);

        // Assert
        Assert.Equal(token, refreshToken.Token);
        Assert.Equal(userId, refreshToken.UserId);
        Assert.Equal(expiresAt, refreshToken.ExpiresAt);
        Assert.False(refreshToken.IsRevoked);
        Assert.True(refreshToken.IsActive);
    }

    [Fact]
    public void Revoke_ActiveToken_MarksTokenRevoked()
    {
        // Arrange
        var refreshToken = RefreshToken.Create("refresh-token", "user-1", DateTime.UtcNow.AddDays(1));

        // Act
        refreshToken.Revoke();

        // Assert
        Assert.True(refreshToken.IsRevoked);
        Assert.NotNull(refreshToken.RevokedAt);
        Assert.False(refreshToken.IsActive);
    }
}
