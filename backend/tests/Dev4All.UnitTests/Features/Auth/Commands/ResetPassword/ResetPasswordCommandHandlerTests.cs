using Dev4All.Application.Abstractions.Auth;
using Dev4All.Application.Features.Auth.Commands.ResetPassword;

namespace Dev4All.UnitTests.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenResetSucceeds_ShouldReturnSuccessResponse()
    {
        var identityService = new FakeIdentityService
        {
            ResetPasswordResult = (true, [])
        };
        var handler = new ResetPasswordCommandHandler(identityService);
        var command = new ResetPasswordCommand("user@example.com", "valid-token", "Valid123!");

        var response = await handler.Handle(command, CancellationToken.None);

        Assert.True(response.Success);
        Assert.Equal("Şifreniz başarıyla sıfırlandı.", response.Message);
        Assert.Equal("user@example.com", identityService.LastEmail);
        Assert.Equal("valid-token", identityService.LastToken);
        Assert.Equal("Valid123!", identityService.LastNewPassword);
    }

    [Fact]
    public async Task Handle_WhenResetFails_ShouldReturnErrorResponse()
    {
        var identityService = new FakeIdentityService
        {
            ResetPasswordResult = (false, ["Invalid token.", "Password is too weak."])
        };
        var handler = new ResetPasswordCommandHandler(identityService);
        var command = new ResetPasswordCommand("user@example.com", "invalid-token", "weak");

        var response = await handler.Handle(command, CancellationToken.None);

        Assert.False(response.Success);
        Assert.Equal("Invalid token., Password is too weak.", response.Message);
    }

    private sealed class FakeIdentityService : IIdentityService
    {
        public (bool Succeeded, IEnumerable<string> Errors) ResetPasswordResult { get; init; }
        public string? LastEmail { get; private set; }
        public string? LastToken { get; private set; }
        public string? LastNewPassword { get; private set; }

        public Task<(bool Succeeded, string UserId, IEnumerable<string> Errors)> CreateUserAsync(string name, string email, string password, string role, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<(bool Succeeded, string UserId, string Email, string Role)> AuthenticateAsync(string email, string password, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<bool> IsInRoleAsync(string userId, string role, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<string?> GetUserNameAsync(string userId, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<string?> GenerateEmailConfirmationTokenAsync(string userId, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<(bool Succeeded, IEnumerable<string> Errors)> ConfirmEmailAsync(string userId, string token, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<string?> GeneratePasswordResetTokenAsync(string email, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<(bool Succeeded, IEnumerable<string> Errors)> ResetPasswordAsync(
            string email,
            string token,
            string newPassword,
            CancellationToken ct = default)
        {
            LastEmail = email;
            LastToken = token;
            LastNewPassword = newPassword;
            return Task.FromResult(ResetPasswordResult);
        }

        public Task<(bool Succeeded, IEnumerable<string> Errors)> ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<string?> GetEmailByUserIdAsync(string userId, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<(string UserId, string Name, bool EmailConfirmed)?> GetUserInfoByEmailAsync(
            string email,
            CancellationToken ct = default)
            => throw new NotImplementedException();
    }
}
