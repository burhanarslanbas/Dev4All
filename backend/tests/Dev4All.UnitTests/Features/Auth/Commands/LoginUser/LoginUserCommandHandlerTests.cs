using Dev4All.Application.Abstractions.Auth;
using Dev4All.Application.Abstractions.Persistence;
using Dev4All.Application.Abstractions.Persistence.Repositories.RefreshTokens;
using Dev4All.Application.Features.Auth.Commands.LoginUser;
using Dev4All.Application.Options;
using Dev4All.Domain.Exceptions;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using RefreshTokenEntity = Dev4All.Domain.Entities.RefreshToken;

namespace Dev4All.UnitTests.Features.Auth.Commands.LoginUser;

public class LoginUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenCredentialsValid_ShouldReturnAccessAndRefreshTokens()
    {
        var identityService = new FakeIdentityService
        {
            AuthenticateResult = (true, "user-1", "user1@example.com", "Customer", true)
        };
        var jwtService = new FakeJwtService
        {
            AccessTokenToReturn = "access-token",
            RefreshTokenToReturn = "refresh-token"
        };
        var repository = new FakeRefreshTokenRepository();
        var unitOfWork = new FakeUnitOfWork();
        var jwtOptions = Options.Create(new JwtOptions { ExpiryInMinutes = 60 });
        var authOptions = Options.Create(new AuthOptions { RefreshTokenLifetimeInDays = 7 });
        var handler = new LoginUserCommandHandler(identityService, jwtService, repository, unitOfWork, jwtOptions, authOptions);

        var response = await handler.Handle(new LoginUserCommand("user1@example.com", "Password123"), CancellationToken.None);

        Assert.Equal("access-token", response.AccessToken);
        Assert.Equal("refresh-token", response.RefreshToken);
        Assert.Equal("user1@example.com", response.Email);
        Assert.Equal("Customer", response.Role);
        Assert.True(response.ExpiresAt > DateTime.UtcNow);
        Assert.NotNull(repository.AddedToken);
        Assert.Equal("refresh-token", repository.AddedToken!.Token);
        Assert.Equal("user-1", repository.AddedToken.UserId);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_WhenCredentialsInvalid_ShouldThrowAuthenticationFailedException()
    {
        var identityService = new FakeIdentityService
        {
            AuthenticateResult = (false, string.Empty, string.Empty, string.Empty, false)
        };
        var handler = BuildHandler(identityService);

        await Assert.ThrowsAsync<AuthenticationFailedException>(() =>
            handler.Handle(new LoginUserCommand("user1@example.com", "WrongPass"), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenEmailNotConfirmed_AndPolicyRequiresConfirmation_ShouldThrowAuthenticationFailedException()
    {
        var identityService = new FakeIdentityService
        {
            AuthenticateResult = (true, "user-1", "user1@example.com", "Customer", false)
        };
        var repository = new FakeRefreshTokenRepository();
        var unitOfWork = new FakeUnitOfWork();
        var jwtOptions = Options.Create(new JwtOptions { ExpiryInMinutes = 60 });
        var authOptions = Options.Create(new AuthOptions
        {
            RefreshTokenLifetimeInDays = 7,
            RequireConfirmedEmail = true
        });
        var handler = new LoginUserCommandHandler(identityService, new FakeJwtService(), repository, unitOfWork, jwtOptions, authOptions);

        await Assert.ThrowsAsync<AuthenticationFailedException>(() =>
            handler.Handle(new LoginUserCommand("user1@example.com", "Password123"), CancellationToken.None));

        Assert.Null(repository.AddedToken);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_WhenEmailNotConfirmed_ButPolicyAllowsIt_ShouldSucceed()
    {
        var identityService = new FakeIdentityService
        {
            AuthenticateResult = (true, "user-1", "user1@example.com", "Customer", false)
        };
        var handler = BuildHandler(identityService);

        var response = await handler.Handle(new LoginUserCommand("user1@example.com", "Password123"), CancellationToken.None);

        Assert.NotNull(response);
        Assert.False(string.IsNullOrWhiteSpace(response.AccessToken));
    }

    private static LoginUserCommandHandler BuildHandler(FakeIdentityService identityService)
    {
        var repository = new FakeRefreshTokenRepository();
        var unitOfWork = new FakeUnitOfWork();
        var jwtOptions = Options.Create(new JwtOptions { ExpiryInMinutes = 60 });
        var authOptions = Options.Create(new AuthOptions
        {
            RefreshTokenLifetimeInDays = 7,
            RequireConfirmedEmail = false
        });
        return new LoginUserCommandHandler(identityService, new FakeJwtService(), repository, unitOfWork, jwtOptions, authOptions);
    }

    private sealed class FakeJwtService : IJwtService
    {
        public string AccessTokenToReturn { get; init; } = "access-token";
        public string RefreshTokenToReturn { get; init; } = "refresh-token";

        public string GenerateToken(string userId, string email, string role) => AccessTokenToReturn;
        public string GenerateRefreshToken() => RefreshTokenToReturn;
        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token) => null;
    }

    private sealed class FakeRefreshTokenRepository : IRefreshTokenRepository
    {
        public RefreshTokenEntity? AddedToken { get; private set; }

        public Task<RefreshTokenEntity?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
            => Task.FromResult<RefreshTokenEntity?>(null);

        public Task AddAsync(RefreshTokenEntity refreshToken, CancellationToken cancellationToken = default)
        {
            AddedToken = refreshToken;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCallCount { get; private set; }

        public Task BeginTransactionAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task CommitTransactionAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task RollbackTransactionAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesCallCount++;
            return Task.FromResult(1);
        }
    }

    private sealed class FakeIdentityService : IIdentityService
    {
        public (bool Succeeded, string UserId, string Email, string Role, bool EmailConfirmed) AuthenticateResult { get; init; }

        public Task<(bool Succeeded, string UserId, IEnumerable<string> Errors)> CreateUserAsync(string name, string email, string password, string role, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<(bool Succeeded, string UserId, string Email, string Role, bool EmailConfirmed)> AuthenticateAsync(string email, string password, CancellationToken ct = default)
            => Task.FromResult(AuthenticateResult);

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
            => throw new NotImplementedException();

        public Task<(bool Succeeded, IEnumerable<string> Errors)> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<(bool Succeeded, IEnumerable<string> Errors)> ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<string?> GetEmailByUserIdAsync(string userId, CancellationToken ct = default)
            => throw new NotImplementedException();
    }
}
