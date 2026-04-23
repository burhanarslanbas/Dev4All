using Dev4All.Application.Abstractions.Auth;
using Dev4All.Application.Abstractions.Persistence;
using Dev4All.Application.Abstractions.Persistence.Repositories.RefreshTokens;
using Dev4All.Application.Features.Auth.Commands.RefreshToken;
using Dev4All.Application.Options;
using Dev4All.Domain.Exceptions;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using RefreshTokenEntity = Dev4All.Domain.Entities.RefreshToken;

namespace Dev4All.UnitTests.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenRequestIsValid_ShouldRotateRefreshTokenAndReturnNewPair()
    {
        var existingToken = RefreshTokenEntity.Create("old-refresh-token", "user-1", DateTime.UtcNow.AddDays(1));
        var jwtService = new FakeJwtService
        {
            Principal = CreatePrincipal("user-1", "Customer"),
            AccessTokenToReturn = "new-access-token",
            RefreshTokenToReturn = "new-refresh-token"
        };
        var identityService = new FakeIdentityService { EmailToReturn = "user1@example.com" };
        var repository = new FakeRefreshTokenRepository { ExistingToken = existingToken };
        var unitOfWork = new FakeUnitOfWork();
        var jwtOptions = Options.Create(new JwtOptions { ExpiryInMinutes = 60 });
        var authOptions = Options.Create(new AuthOptions { RefreshTokenLifetimeInDays = 7 });
        var handler = new RefreshTokenCommandHandler(jwtService, identityService, repository, unitOfWork, jwtOptions, authOptions);

        var response = await handler.Handle(new RefreshTokenCommand("expired-access", "old-refresh-token"), CancellationToken.None);

        Assert.Equal("new-access-token", response.AccessToken);
        Assert.Equal("new-refresh-token", response.RefreshToken);
        Assert.Equal("user1@example.com", response.Email);
        Assert.Equal("Customer", response.Role);
        Assert.True(response.ExpiresAt > DateTime.UtcNow);
        Assert.True(existingToken.IsRevoked);
        Assert.NotNull(repository.AddedToken);
        Assert.Equal("new-refresh-token", repository.AddedToken!.Token);
        Assert.Equal("user-1", repository.AddedToken.UserId);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_WhenRefreshTokenIsInvalid_ShouldThrowAuthenticationFailedException()
    {
        var jwtService = new FakeJwtService
        {
            Principal = CreatePrincipal("user-1", "Customer")
        };
        var identityService = new FakeIdentityService { EmailToReturn = "user1@example.com" };
        var repository = new FakeRefreshTokenRepository { ExistingToken = null };
        var unitOfWork = new FakeUnitOfWork();
        var jwtOptions = Options.Create(new JwtOptions { ExpiryInMinutes = 60 });
        var authOptions = Options.Create(new AuthOptions { RefreshTokenLifetimeInDays = 7 });
        var handler = new RefreshTokenCommandHandler(jwtService, identityService, repository, unitOfWork, jwtOptions, authOptions);

        await Assert.ThrowsAsync<AuthenticationFailedException>(() =>
            handler.Handle(new RefreshTokenCommand("expired-access", "missing-token"), CancellationToken.None));

        Assert.Null(repository.AddedToken);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    private static ClaimsPrincipal CreatePrincipal(string userId, string role)
    {
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Role, role)
        ], "TestAuth");

        return new ClaimsPrincipal(identity);
    }

    private sealed class FakeJwtService : IJwtService
    {
        public ClaimsPrincipal? Principal { get; init; }
        public string AccessTokenToReturn { get; init; } = "generated-access";
        public string RefreshTokenToReturn { get; init; } = "generated-refresh";

        public string GenerateToken(string userId, string email, string role) => AccessTokenToReturn;
        public string GenerateRefreshToken() => RefreshTokenToReturn;
        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token) => Principal;
    }

    private sealed class FakeRefreshTokenRepository : IRefreshTokenRepository
    {
        public RefreshTokenEntity? ExistingToken { get; init; }
        public RefreshTokenEntity? AddedToken { get; private set; }

        public Task<RefreshTokenEntity?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
            => Task.FromResult(ExistingToken);

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
        public string? EmailToReturn { get; init; }

        public Task<(bool Succeeded, string UserId, IEnumerable<string> Errors)> CreateUserAsync(string name, string email, string password, string role, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<(bool Succeeded, string UserId, string Email, string Role, bool EmailConfirmed)> AuthenticateAsync(string email, string password, CancellationToken ct = default)
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
            => throw new NotImplementedException();

        public Task<(bool Succeeded, IEnumerable<string> Errors)> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<(bool Succeeded, IEnumerable<string> Errors)> ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<string?> GetEmailByUserIdAsync(string userId, CancellationToken ct = default)
            => Task.FromResult(EmailToReturn);
    }
}
