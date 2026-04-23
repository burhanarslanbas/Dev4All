using Dev4All.Application.Abstractions.Auth;
using Dev4All.Application.Abstractions.Services;
using Dev4All.Application.Features.Auth.Commands.RegisterUser;
using Dev4All.Domain.Enums;
using Dev4All.Domain.Exceptions;

namespace Dev4All.UnitTests.Features.Auth.Commands.RegisterUser;

public class RegisterUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenRegistrationSucceeds_ShouldQueueWelcomeAndConfirmationEmails()
    {
        var userId = Guid.NewGuid();
        var identityService = new FakeIdentityService
        {
            CreateResult = (true, userId.ToString(), []),
            ConfirmationTokenToReturn = "confirm-token"
        };
        var email = new FakeEmailNotificationService();
        var handler = new RegisterUserCommandHandler(identityService, email);

        var response = await handler.Handle(
            new RegisterUserCommand("Alice", "alice@example.com", "Password123", UserRole.Customer),
            CancellationToken.None);

        Assert.Equal(userId, response.UserId);
        Assert.Equal("alice@example.com", response.Email);
        Assert.Equal("Alice", response.Name);

        Assert.Equal(1, email.WelcomeCallCount);
        Assert.Equal("alice@example.com", email.LastWelcomeEmail);
        Assert.Equal(1, email.ConfirmationCallCount);
        Assert.Equal(userId.ToString(), email.LastConfirmationUserId);
        Assert.Equal("confirm-token", email.LastConfirmationToken);
    }

    [Fact]
    public async Task Handle_WhenRoleIsAdmin_ShouldThrowUnauthorizedDomainException()
    {
        var handler = new RegisterUserCommandHandler(new FakeIdentityService(), new FakeEmailNotificationService());

        await Assert.ThrowsAsync<UnauthorizedDomainException>(() =>
            handler.Handle(
                new RegisterUserCommand("Alice", "alice@example.com", "Password123", UserRole.Admin),
                CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenIdentityServiceFails_ShouldThrowBusinessRuleViolationException()
    {
        var identityService = new FakeIdentityService
        {
            CreateResult = (false, string.Empty, ["Email already taken."])
        };
        var handler = new RegisterUserCommandHandler(identityService, new FakeEmailNotificationService());

        var ex = await Assert.ThrowsAsync<BusinessRuleViolationException>(() =>
            handler.Handle(
                new RegisterUserCommand("Alice", "alice@example.com", "Password123", UserRole.Customer),
                CancellationToken.None));

        Assert.Contains("Email already taken.", ex.Message);
    }

    private sealed class FakeIdentityService : IIdentityService
    {
        public (bool Succeeded, string UserId, IEnumerable<string> Errors) CreateResult { get; init; } = (true, Guid.NewGuid().ToString(), []);
        public string? ConfirmationTokenToReturn { get; init; }

        public Task<(bool Succeeded, string UserId, IEnumerable<string> Errors)> CreateUserAsync(string name, string email, string password, string role, CancellationToken ct = default)
            => Task.FromResult(CreateResult);

        public Task<(bool Succeeded, string UserId, string Email, string Role, bool EmailConfirmed)> AuthenticateAsync(string email, string password, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<bool> IsInRoleAsync(string userId, string role, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<string?> GetUserNameAsync(string userId, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<(string? UserId, string? Name, bool EmailConfirmed)> GetUserByEmailAsync(string email, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<string?> GenerateEmailConfirmationTokenAsync(string userId, CancellationToken ct = default)
            => Task.FromResult(ConfirmationTokenToReturn);

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
        public int WelcomeCallCount { get; private set; }
        public int ConfirmationCallCount { get; private set; }
        public string? LastWelcomeEmail { get; private set; }
        public string? LastConfirmationUserId { get; private set; }
        public string? LastConfirmationToken { get; private set; }

        public Task QueueWelcomeEmailAsync(string email, string name, CancellationToken ct = default)
        {
            WelcomeCallCount++;
            LastWelcomeEmail = email;
            return Task.CompletedTask;
        }

        public Task QueueConfirmationEmailAsync(string userId, string email, string name, string token, CancellationToken ct = default)
        {
            ConfirmationCallCount++;
            LastConfirmationUserId = userId;
            LastConfirmationToken = token;
            return Task.CompletedTask;
        }

        public Task QueuePasswordResetEmailAsync(string email, string resetUrl, CancellationToken ct = default)
            => Task.CompletedTask;

        public Task QueueChangePasswordSuccessEmailAsync(string email, string name, CancellationToken ct = default)
            => Task.CompletedTask;
    }
}
