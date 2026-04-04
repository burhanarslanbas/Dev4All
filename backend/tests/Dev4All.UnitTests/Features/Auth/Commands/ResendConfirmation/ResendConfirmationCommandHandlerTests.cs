using Dev4All.Application.Abstractions.Auth;
using Dev4All.Application.Abstractions.Services;
using Dev4All.Application.Features.Auth.Commands.ResendConfirmation;

namespace Dev4All.UnitTests.Features.Auth.Commands.ResendConfirmation;

public class ResendConfirmationCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserExistsAndEmailNotConfirmed_QueuesConfirmationEmail()
    {
        var identityService = new FakeIdentityService
        {
            UserInfoToReturn = ("user-1", "Test User", false),
            TokenToReturn = "confirmation-token"
        };
        var emailNotificationService = new FakeEmailNotificationService();
        var handler = new ResendConfirmationCommandHandler(identityService, emailNotificationService);

        var response = await handler.Handle(new ResendConfirmationCommand("user@example.com"), CancellationToken.None);

        Assert.Equal("Hesap mevcutsa doğrulama e-postası yeniden gönderildi.", response.Message);
        Assert.Equal("user-1", identityService.GenerateTokenUserId);
        Assert.Equal(1, emailNotificationService.QueueCallCount);
        Assert.Equal("user@example.com", emailNotificationService.Email);
        Assert.Equal("Test User", emailNotificationService.Name);
        Assert.Equal("confirmation-token", emailNotificationService.Token);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ReturnsGenericMessageWithoutQueue()
    {
        var identityService = new FakeIdentityService
        {
            UserInfoToReturn = null
        };
        var emailNotificationService = new FakeEmailNotificationService();
        var handler = new ResendConfirmationCommandHandler(identityService, emailNotificationService);

        var response = await handler.Handle(new ResendConfirmationCommand("missing@example.com"), CancellationToken.None);

        Assert.Equal("Hesap mevcutsa doğrulama e-postası yeniden gönderildi.", response.Message);
        Assert.Null(identityService.GenerateTokenUserId);
        Assert.Equal(0, emailNotificationService.QueueCallCount);
    }

    [Fact]
    public async Task Handle_WhenEmailAlreadyConfirmed_ReturnsGenericMessageWithoutQueue()
    {
        var identityService = new FakeIdentityService
        {
            UserInfoToReturn = ("user-2", "Confirmed User", true)
        };
        var emailNotificationService = new FakeEmailNotificationService();
        var handler = new ResendConfirmationCommandHandler(identityService, emailNotificationService);

        var response = await handler.Handle(new ResendConfirmationCommand("confirmed@example.com"), CancellationToken.None);

        Assert.Equal("Hesap mevcutsa doğrulama e-postası yeniden gönderildi.", response.Message);
        Assert.Null(identityService.GenerateTokenUserId);
        Assert.Equal(0, emailNotificationService.QueueCallCount);
    }

    private sealed class FakeIdentityService : IIdentityService
    {
        public (string UserId, string Name, bool EmailConfirmed)? UserInfoToReturn { get; init; }
        public string? TokenToReturn { get; init; }
        public string? GenerateTokenUserId { get; private set; }

        public Task<(bool Succeeded, string UserId, IEnumerable<string> Errors)> CreateUserAsync(string name, string email, string password, string role, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<(bool Succeeded, string UserId, string Email, string Role)> AuthenticateAsync(string email, string password, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<bool> IsInRoleAsync(string userId, string role, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<string?> GetUserNameAsync(string userId, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<string?> GenerateEmailConfirmationTokenAsync(string userId, CancellationToken ct = default)
        {
            GenerateTokenUserId = userId;
            return Task.FromResult(TokenToReturn);
        }

        public Task<(bool Succeeded, IEnumerable<string> Errors)> ConfirmEmailAsync(string userId, string token, CancellationToken ct = default)
            => throw new NotImplementedException();

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
            => Task.FromResult(UserInfoToReturn);
    }

    private sealed class FakeEmailNotificationService : IEmailNotificationService
    {
        public int QueueCallCount { get; private set; }
        public string? Email { get; private set; }
        public string? Name { get; private set; }
        public string? Token { get; private set; }

        public Task QueueConfirmationEmailAsync(string email, string name, string token, CancellationToken ct = default)
        {
            QueueCallCount++;
            Email = email;
            Name = name;
            Token = token;
            return Task.CompletedTask;
        }

        public Task QueuePasswordResetEmailAsync(string email, string resetUrl, CancellationToken ct = default)
            => throw new NotImplementedException();
    }
}
