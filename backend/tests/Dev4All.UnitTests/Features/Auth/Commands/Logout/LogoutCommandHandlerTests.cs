using Dev4All.Application.Abstractions.Persistence;
using Dev4All.Application.Abstractions.Persistence.Repositories.RefreshTokens;
using Dev4All.Application.Features.Auth.Commands.Logout;
using RefreshTokenEntity = global::Dev4All.Domain.Entities.RefreshToken;

namespace Dev4All.UnitTests.Features.Auth.Commands.Logout;

public class LogoutCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenTokenExistsAndNotRevoked_RevokesAndSaves()
    {
        var token = RefreshTokenEntity.Create("refresh-token", "user-1", DateTime.UtcNow.AddDays(1));
        var writeRepository = new FakeRefreshTokenWriteRepository(token);
        var unitOfWork = new FakeUnitOfWork();
        var handler = new LogoutCommandHandler(writeRepository, unitOfWork);

        var result = await handler.Handle(new LogoutCommand("refresh-token"), CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal("Çıkış işlemi başarılı.", result.Message);
        Assert.True(token.IsRevoked);
        Assert.Equal(1, writeRepository.UpdateCallCount);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_WhenTokenDoesNotExist_DoesNotUpdateOrSave()
    {
        var writeRepository = new FakeRefreshTokenWriteRepository();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new LogoutCommandHandler(writeRepository, unitOfWork);

        var result = await handler.Handle(new LogoutCommand("missing-token"), CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal("Çıkış işlemi başarılı.", result.Message);
        Assert.Equal(0, writeRepository.UpdateCallCount);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_WhenTokenAlreadyRevoked_DoesNotUpdateOrSave()
    {
        var token = RefreshTokenEntity.Create("refresh-token", "user-1", DateTime.UtcNow.AddDays(1));
        token.Revoke();
        var writeRepository = new FakeRefreshTokenWriteRepository(token);
        var unitOfWork = new FakeUnitOfWork();
        var handler = new LogoutCommandHandler(writeRepository, unitOfWork);

        var result = await handler.Handle(new LogoutCommand("refresh-token"), CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal("Çıkış işlemi başarılı.", result.Message);
        Assert.Equal(0, writeRepository.UpdateCallCount);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    private sealed class FakeRefreshTokenWriteRepository : IRefreshTokenWriteRepository
    {
        private readonly RefreshTokenEntity? _refreshToken;
        public int UpdateCallCount { get; private set; }

        public FakeRefreshTokenWriteRepository(RefreshTokenEntity? refreshToken = null)
        {
            _refreshToken = refreshToken;
        }

        public Task AddAsync(RefreshTokenEntity entity, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task AddRangeAsync(IEnumerable<RefreshTokenEntity> entities, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public void Update(RefreshTokenEntity entity)
            => UpdateCallCount++;

        public void UpdateRange(IEnumerable<RefreshTokenEntity> entities)
        {
        }

        public void Remove(RefreshTokenEntity entity)
        {
        }

        public void RemoveRange(IEnumerable<RefreshTokenEntity> entities)
        {
        }

        public Task<RefreshTokenEntity?> GetByTokenForUpdateAsync(string token, CancellationToken cancellationToken = default)
            => Task.FromResult<RefreshTokenEntity?>(_refreshToken);
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCallCount { get; private set; }

        public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesCallCount++;
            return Task.FromResult(1);
        }
    }
}
