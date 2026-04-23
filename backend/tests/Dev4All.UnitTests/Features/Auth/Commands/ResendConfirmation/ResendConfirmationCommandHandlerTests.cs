using Dev4All.Application.Abstractions.Auth;
using Dev4All.Application.Abstractions.Services;
using Dev4All.Application.Features.Auth.Commands.ResendConfirmation;

namespace Dev4All.UnitTests.Features.Auth.Commands.ResendConfirmation;

public class ResendConfirmationCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserExistsAndEmailNotConfirmed_ShouldQueueConfirmationEmailAndReturnGenericMessage()
    {
        var identityService = new FakeIdentityService
        {
            UserByEmailResult = ("user-1", "User One", false),
            GeneratedToken = "token-123"
        };
        var emailNotificationService = new FakeEmailNotificationService();
        var handler = new ResendConfirmationCommandHandler(identityService, emailNotificationService);

        var response = await handler.Handle(new ResendConfirmationCommand("user@example.com"), CancellationToken.None);

        Assert.Equal("Eğer hesap mevcutsa, e-posta doğrulama bağlantısı yeniden gönderilecektir.", response.Message);
        Assert.Equal("user@example.com", identityService.LastEmail);
        Assert.Equal("user-1", identityService.LastTokenUserId);
        Assert.Equal(1, emailNotificationService.CallCount);
        Assert.Equal("user-1", emailNotificationService.LastUserId);
        Assert.Equal("user@example.com", emailNotificationService.LastEmail);
        Assert.Equal("User One", emailNotificationService.LastName);
        Assert.Equal("token-123", emailNotificationService.LastToken);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ShouldNotQueueEmailAndReturnGenericMessage()
    {
        var identityService = new FakeIdentityService
        {
            UserByEmailResult = (null, null, false),
            GeneratedToken = "token-123"
        };
        var emailNotificationService = new FakeEmailNotificationService();
        var handler = new ResendConfirmationCommandHandler(identityService, emailNotificationService);

        var response = await handler.Handle(new ResendConfirmationCommand("missing@example.com"), CancellationToken.None);

        Assert.Equal("Eğer hesap mevcutsa, e-posta doğrulama bağlantısı yeniden gönderilecektir.", response.Message);
        Assert.Equal("missing@example.com", identityService.LastEmail);
        Assert.Null(identityService.LastTokenUserId);
        Assert.Equal(0, emailNotificationService.CallCount);
    }

    [Fact]
    public async Task Handle_WhenEmailAlreadyConfirmed_ShouldNotQueueEmailAndReturnGenericMessage()
    {
        var identityService = new FakeIdentityService
        {
            UserByEmailResult = ("user-1", "User One", true),
            GeneratedToken = "token-123"
        };
        var emailNotificationService = new FakeEmailNotificationService();
        var handler = new ResendConfirmationCommandHandler(identityService, emailNotificationService);

        var response = await handler.Handle(new ResendConfirmationCommand("user@example.com"), CancellationToken.None);

        Assert.Equal("Eğer hesap mevcutsa, e-posta doğrulama bağlantısı yeniden gönderilecektir.", response.Message);
        Assert.Equal("user@example.com", identityService.LastEmail);
        Assert.Null(identityService.LastTokenUserId);
        Assert.Equal(0, emailNotificationService.CallCount);
    }

    [Fact]
    public async Task Handle_WhenNameIsMissing_ShouldQueueEmailWithDefaultRecipientName()
    {
        var identityService = new FakeIdentityService
        {
            UserByEmailResult = ("user-1", null, false),
            GeneratedToken = "token-123"
        };
        var emailNotificationService = new FakeEmailNotificationService();
        var handler = new ResendConfirmationCommandHandler(identityService, emailNotificationService);

        var response = await handler.Handle(new ResendConfirmationCommand("user@example.com"), CancellationToken.None);

        Assert.Equal("Eğer hesap mevcutsa, e-posta doğrulama bağlantısı yeniden gönderilecektir.", response.Message);
        Assert.Equal(1, emailNotificationService.CallCount);
        Assert.Equal("User", emailNotificationService.LastName);
    }

    private sealed class FakeIdentityService : IIdentityService
    {
        public (string? UserId, string? Name, bool EmailConfirmed) UserByEmailResult { get; init; }
        public string? GeneratedToken { get; init; }
        public string? LastEmail { get; private set; }
        public string? LastTokenUserId { get; private set; }

        public Task<(bool Succeeded, string UserId, IEnumerable<string> Errors)> CreateUserAsync(string name, string email, string password, string role, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<(bool Succeeded, string UserId, string Email, string Role, bool EmailConfirmed)> AuthenticateAsync(string email, string password, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<bool> IsInRoleAsync(string userId, string role, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<string?> GetUserNameAsync(string userId, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<(string? UserId, string? Name, bool EmailConfirmed)> GetUserByEmailAsync(string email, CancellationToken ct = default)
        {
            LastEmail = email;
            return Task.FromResult(UserByEmailResult);
        }

        public Task<string?> GenerateEmailConfirmationTokenAsync(string userId, CancellationToken ct = default)
        {
            LastTokenUserId = userId;
            return Task.FromResult(GeneratedToken);
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
    }

    private sealed class FakeEmailNotificationService : IEmailNotificationService
    {
        public int CallCount { get; private set; }
        public string? LastUserId { get; private set; }
        public string? LastEmail { get; private set; }
        public string? LastName { get; private set; }
        public string? LastToken { get; private set; }

        public Task QueueChangePasswordSuccessEmailAsync(string email, string name, CancellationToken ct = default)
            => Task.CompletedTask;

        public Task QueuePasswordResetEmailAsync(string email, string resetUrl, CancellationToken ct = default)
            => Task.CompletedTask;

        public Task QueueConfirmationEmailAsync(string userId, string email, string name, string token, CancellationToken ct = default)
        {
            CallCount++;
            LastUserId = userId;
            LastEmail = email;
            LastName = name;
            LastToken = token;
            return Task.CompletedTask;
        }

        public Task QueueWelcomeEmailAsync(string email, string name, CancellationToken ct = default)
            => Task.CompletedTask;
    }
}
