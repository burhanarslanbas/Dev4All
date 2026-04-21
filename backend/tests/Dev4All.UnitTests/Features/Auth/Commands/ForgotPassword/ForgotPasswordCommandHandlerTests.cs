using Dev4All.Application.Abstractions.Auth;
using Dev4All.Application.Abstractions.Services;
using Dev4All.Application.Features.Auth.Commands.ForgotPassword;

namespace Dev4All.UnitTests.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserExists_ShouldQueuePasswordResetEmailAndReturnGenericMessage()
    {
        var identityService = new FakeIdentityService { ResetTokenToReturn = "token-value" };
        var emailNotificationService = new FakeEmailNotificationService();
        var handler = new ForgotPasswordCommandHandler(identityService, emailNotificationService);
        var command = new ForgotPasswordCommand("user@example.com");

        var response = await handler.Handle(command, CancellationToken.None);

        Assert.Equal("Eğer bu e-posta sistemde kayıtlıysa, şifre sıfırlama bağlantısı gönderilecektir.", response.Message);
        Assert.Equal("user@example.com", identityService.LastEmail);
        Assert.Equal(1, emailNotificationService.CallCount);
        Assert.Equal("user@example.com", emailNotificationService.LastEmail);
        Assert.Contains("email=user%40example.com", emailNotificationService.LastResetUrl);
        Assert.Contains("token=token-value", emailNotificationService.LastResetUrl);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ShouldNotQueueEmailAndReturnGenericMessage()
    {
        var identityService = new FakeIdentityService { ResetTokenToReturn = null };
        var emailNotificationService = new FakeEmailNotificationService();
        var handler = new ForgotPasswordCommandHandler(identityService, emailNotificationService);
        var command = new ForgotPasswordCommand("missing@example.com");

        var response = await handler.Handle(command, CancellationToken.None);

        Assert.Equal("Eğer bu e-posta sistemde kayıtlıysa, şifre sıfırlama bağlantısı gönderilecektir.", response.Message);
        Assert.Equal("missing@example.com", identityService.LastEmail);
        Assert.Equal(0, emailNotificationService.CallCount);
    }

    private sealed class FakeIdentityService : IIdentityService
    {
        public string? ResetTokenToReturn { get; init; }
        public string? LastEmail { get; private set; }

        public Task<(bool Succeeded, string UserId, IEnumerable<string> Errors)> CreateUserAsync(string name, string email, string password, string role, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<(bool Succeeded, string UserId, string Email, string Role)> AuthenticateAsync(string email, string password, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<bool> IsInRoleAsync(string userId, string role, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<string?> GetUserNameAsync(string userId, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<(string? UserId, string? Name, bool EmailConfirmed)> GetUserByEmailAsync(string email, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<string?> GenerateEmailConfirmationTokenAsync(string userId, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<(bool Succeeded, IEnumerable<string> Errors)> ConfirmEmailAsync(string userId, string token, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<string?> GeneratePasswordResetTokenAsync(string email, CancellationToken ct = default)
        {
            LastEmail = email;
            return Task.FromResult(ResetTokenToReturn);
        }

        public Task<(bool Succeeded, IEnumerable<string> Errors)> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<(bool Succeeded, IEnumerable<string> Errors)> ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<string?> GetEmailByUserIdAsync(string userId, CancellationToken ct = default)
            => throw new NotImplementedException();
    }

    private sealed class FakeEmailNotificationService : IEmailNotificationService
    {
        public int CallCount { get; private set; }
        public string LastEmail { get; private set; } = string.Empty;
        public string LastResetUrl { get; private set; } = string.Empty;

        public Task QueuePasswordResetEmailAsync(string email, string resetUrl, CancellationToken ct = default)
        {
            CallCount++;
            LastEmail = email;
            LastResetUrl = resetUrl;
            return Task.CompletedTask;
        }

        public Task QueueChangePasswordSuccessEmailAsync(string email, string name, CancellationToken ct = default)
            => Task.CompletedTask;

        public Task QueueConfirmationEmailAsync(string email, string name, string token, CancellationToken ct = default)
            => Task.CompletedTask;
    }
}
