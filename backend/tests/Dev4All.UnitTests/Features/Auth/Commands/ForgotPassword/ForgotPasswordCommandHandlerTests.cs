using Dev4All.Application.Abstractions.Auth;
using Dev4All.Application.Abstractions.Services;
using Dev4All.Application.Features.Auth.Commands.ForgotPassword;
using Dev4All.Application.Options;
using Microsoft.Extensions.Options;

namespace Dev4All.UnitTests.Features.Auth.Commands.ForgotPassword;

public sealed class ForgotPasswordCommandHandlerTests
{
    [Fact]
    public async Task HandleWhenUserExistsQueuesPasswordResetEmailAndReturnsGenericMessage()
    {
        // Arrange
        var identityService = new FakeIdentityService
        {
            Response = (true, "john.doe", "token+with/slash")
        };
        var emailNotificationService = new FakeEmailNotificationService();
        var options = Options.Create(new FrontendOptions { PasswordResetUrl = "https://dev4all.app/reset-password" });
        var handler = new ForgotPasswordCommandHandler(identityService, emailNotificationService, options);
        var command = new ForgotPasswordCommand("john.doe+test@dev4all.app");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("Eğer bu e-posta adresi sistemde kayıtlıysa, şifre sıfırlama bağlantısı gönderilecektir.", result.Message);
        var call = Assert.Single(emailNotificationService.Calls);

        Assert.Equal(command.Email, call.Email);
        Assert.Equal("john.doe", call.UserName);
        Assert.Equal("token+with/slash", call.ResetToken);
        Assert.Equal(
            "https://dev4all.app/reset-password?email=john.doe%2Btest%40dev4all.app&token=token%2Bwith%2Fslash",
            call.RedirectUrl);
    }

    [Fact]
    public async Task HandleWhenUserDoesNotExistDoesNotQueueEmailAndReturnsGenericMessage()
    {
        // Arrange
        var identityService = new FakeIdentityService
        {
            Response = (false, string.Empty, string.Empty)
        };
        var emailNotificationService = new FakeEmailNotificationService();
        var options = Options.Create(new FrontendOptions { PasswordResetUrl = "https://dev4all.app/reset-password" });
        var handler = new ForgotPasswordCommandHandler(identityService, emailNotificationService, options);
        var command = new ForgotPasswordCommand("missing@dev4all.app");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("Eğer bu e-posta adresi sistemde kayıtlıysa, şifre sıfırlama bağlantısı gönderilecektir.", result.Message);
        Assert.Empty(emailNotificationService.Calls);
    }

    private sealed class FakeIdentityService : IIdentityService
    {
        public (bool UserExists, string UserName, string ResetToken) Response { get; set; }

        public Task<(bool Succeeded, string UserId, IEnumerable<string> Errors)> CreateUserAsync(
            string name,
            string email,
            string password,
            string role,
            CancellationToken ct = default) => throw new NotSupportedException();

        public Task<(bool Succeeded, string UserId, string Email, string Role)> AuthenticateAsync(
            string email,
            string password,
            CancellationToken ct = default) => throw new NotSupportedException();

        public Task<bool> IsInRoleAsync(string userId, string role, CancellationToken ct = default)
            => throw new NotSupportedException();

        public Task<string?> GetUserNameAsync(string userId, CancellationToken ct = default)
            => throw new NotSupportedException();

        public Task<(bool UserExists, string UserName, string ResetToken)> GeneratePasswordResetTokenAsync(
            string email,
            CancellationToken ct = default)
            => Task.FromResult(Response);
    }

    private sealed class FakeEmailNotificationService : IEmailNotificationService
    {
        public List<(string Email, string UserName, string ResetToken, string RedirectUrl)> Calls { get; } = [];

        public Task QueuePasswordResetEmailAsync(
            string email,
            string userName,
            string resetToken,
            string redirectUrl,
            CancellationToken ct = default)
        {
            Calls.Add((email, userName, resetToken, redirectUrl));
            return Task.CompletedTask;
        }
    }
}
