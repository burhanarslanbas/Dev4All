using Dev4All.Application.Abstractions.Auth;
using Dev4All.Application.Abstractions.Services;
using Dev4All.Application.Features.Auth.Commands.ChangePassword;
using Dev4All.Domain.Exceptions;

namespace Dev4All.UnitTests.Features.Auth.Commands.ChangePassword;

public class ChangePasswordCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenCurrentUserIsNotAuthenticated_ShouldThrowUnauthorizedException()
    {
        var currentUser = new FakeCurrentUser(false, string.Empty, string.Empty, string.Empty);
        var identityService = new FakeIdentityService();
        var emailNotificationService = new FakeEmailNotificationService();
        var handler = new ChangePasswordCommandHandler(currentUser, identityService, emailNotificationService);

        await Assert.ThrowsAsync<AuthenticationFailedException>(() =>
            handler.Handle(new ChangePasswordCommand("Current123", "NewPassword1"), CancellationToken.None));

        Assert.Equal(0, emailNotificationService.QueueCallCount);
    }

    [Fact]
    public async Task Handle_WhenIdentityChangePasswordFails_ShouldReturnFailureResponse()
    {
        var currentUser = new FakeCurrentUser(true, "user-1", "user1@example.com", "Customer");
        var identityService = new FakeIdentityService
        {
            EmailToReturn = "user1@example.com",
            ChangePasswordResult = (false, ["Current password is invalid."])
        };
        var emailNotificationService = new FakeEmailNotificationService();
        var handler = new ChangePasswordCommandHandler(currentUser, identityService, emailNotificationService);

        var response = await handler.Handle(new ChangePasswordCommand("WrongCurrent123", "NewPassword1"), CancellationToken.None);

        Assert.False(response.Success);
        Assert.Equal("Current password is invalid.", response.Message);
        Assert.Equal(0, emailNotificationService.QueueCallCount);
    }

    [Fact]
    public async Task Handle_WhenIdentityChangePasswordSucceeds_ShouldQueueSuccessEmailAndReturnSuccess()
    {
        var currentUser = new FakeCurrentUser(true, "user-1", "user1@example.com", "Customer");
        var identityService = new FakeIdentityService
        {
            EmailToReturn = "user1@example.com",
            UserNameToReturn = "User One",
            ChangePasswordResult = (true, [])
        };
        var emailNotificationService = new FakeEmailNotificationService();
        var handler = new ChangePasswordCommandHandler(currentUser, identityService, emailNotificationService);

        var response = await handler.Handle(new ChangePasswordCommand("Current123", "NewPassword1"), CancellationToken.None);

        Assert.True(response.Success);
        Assert.Equal("Şifre başarıyla değiştirildi.", response.Message);
        Assert.Equal(1, emailNotificationService.QueueCallCount);
        Assert.Equal("user1@example.com", emailNotificationService.LastEmail);
        Assert.Equal("User One", emailNotificationService.LastName);
        Assert.Equal("user-1", identityService.LastChangedPasswordUserId);
    }

    private sealed class FakeCurrentUser(bool isAuthenticated, string userId, string email, string role) : ICurrentUser
    {
        public string UserId { get; } = userId;
        public string Email { get; } = email;
        public string Role { get; } = role;
        public bool IsAuthenticated { get; } = isAuthenticated;
    }

    private sealed class FakeEmailNotificationService : IEmailNotificationService
    {
        public int QueueCallCount { get; private set; }
        public string? LastEmail { get; private set; }
        public string? LastName { get; private set; }

        public Task QueueChangePasswordSuccessEmailAsync(string email, string name, CancellationToken ct = default)
        {
            QueueCallCount++;
            LastEmail = email;
            LastName = name;
            return Task.CompletedTask;
        }

        public Task QueuePasswordResetEmailAsync(string email, string resetUrl, CancellationToken ct = default)
            => Task.CompletedTask;

        public Task QueueConfirmationEmailAsync(string userId, string email, string name, string token, CancellationToken ct = default)
            => Task.CompletedTask;

        public Task QueueWelcomeEmailAsync(string email, string name, CancellationToken ct = default)
            => Task.CompletedTask;
    }

    private sealed class FakeIdentityService : IIdentityService
    {
        public string? EmailToReturn { get; init; }
        public string? UserNameToReturn { get; init; }
        public (bool Succeeded, IEnumerable<string> Errors) ChangePasswordResult { get; init; } = (true, []);
        public string? LastChangedPasswordUserId { get; private set; }

        public Task<(bool Succeeded, string UserId, IEnumerable<string> Errors)> CreateUserAsync(string name, string email, string password, string role, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<(bool Succeeded, string UserId, string Email, string Role, bool EmailConfirmed)> AuthenticateAsync(string email, string password, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<bool> IsInRoleAsync(string userId, string role, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<string?> GetUserNameAsync(string userId, CancellationToken ct = default)
            => Task.FromResult(UserNameToReturn);

        public Task<(string? UserId, string? Name, bool EmailConfirmed)> GetUserByEmailAsync(string email, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<string?> GenerateEmailConfirmationTokenAsync(string userId, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<(bool Succeeded, IEnumerable<string> Errors)> ConfirmEmailAsync(string userId, string token, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<string?> GeneratePasswordResetTokenAsync(string email, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<(bool Succeeded, IEnumerable<string> Errors)> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<(bool Succeeded, IEnumerable<string> Errors)> ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken ct = default)
        {
            LastChangedPasswordUserId = userId;
            return Task.FromResult(ChangePasswordResult);
        }

        public Task<string?> GetEmailByUserIdAsync(string userId, CancellationToken ct = default)
            => Task.FromResult(EmailToReturn);
    }
}
