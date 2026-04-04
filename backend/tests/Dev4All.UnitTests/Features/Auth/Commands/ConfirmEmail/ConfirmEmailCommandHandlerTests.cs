using Dev4All.Application.Abstractions.Auth;
using Dev4All.Application.Features.Auth.Commands.ConfirmEmail;

namespace Dev4All.UnitTests.Features.Auth.Commands.ConfirmEmail;

public class ConfirmEmailCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenIdentityConfirmationSucceeds_ShouldReturnSuccessResponse()
    {
        var identityService = new FakeIdentityService { ConfirmationResult = (true, []) };
        var handler = new ConfirmEmailCommandHandler(identityService);

        var response = await handler.Handle(new ConfirmEmailCommand("user-1", "token-1"), CancellationToken.None);

        Assert.True(response.Success);
        Assert.Equal("E-posta doğrulandı.", response.Message);
        Assert.Equal("user-1", identityService.LastUserId);
        Assert.Equal("token-1", identityService.LastToken);
    }

    [Fact]
    public async Task Handle_WhenIdentityConfirmationFails_ShouldReturnFailureResponse()
    {
        var identityService = new FakeIdentityService { ConfirmationResult = (false, ["Invalid token"]) };
        var handler = new ConfirmEmailCommandHandler(identityService);

        var response = await handler.Handle(new ConfirmEmailCommand("user-1", "token-1"), CancellationToken.None);

        Assert.False(response.Success);
        Assert.Equal("E-posta doğrulama işlemi başarısız.", response.Message);
        Assert.Equal("user-1", identityService.LastUserId);
        Assert.Equal("token-1", identityService.LastToken);
    }

    private sealed class FakeIdentityService : IIdentityService
    {
        public (bool Succeeded, IEnumerable<string> Errors) ConfirmationResult { get; init; }
        public string? LastUserId { get; private set; }
        public string? LastToken { get; private set; }

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
        {
            LastUserId = userId;
            LastToken = token;
            return Task.FromResult(ConfirmationResult);
        }

        public Task<string?> GeneratePasswordResetTokenAsync(string email, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<(bool Succeeded, IEnumerable<string> Errors)> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken ct = default)
            => throw new NotImplementedException();

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
