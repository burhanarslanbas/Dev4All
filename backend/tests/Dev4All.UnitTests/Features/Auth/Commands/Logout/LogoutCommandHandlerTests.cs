using Dev4All.Application.Abstractions.Persistence;
using Dev4All.Application.Abstractions.Persistence.Repositories.RefreshTokens;
using Dev4All.Application.Common.Pagination;
using Dev4All.Application.Features.Auth.Commands.Logout;
using Dev4All.Domain.Entities;

namespace Dev4All.UnitTests.Features.Auth.Commands.Logout;

public class LogoutCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenTokenExistsAndNotRevoked_RevokesAndSaves()
    {
        var token = RefreshToken.Create("refresh-token", "user-1", DateTime.UtcNow.AddDays(1));
        var readRepository = new FakeRefreshTokenReadRepository(token);
        var writeRepository = new FakeRefreshTokenWriteRepository();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new LogoutCommandHandler(readRepository, writeRepository, unitOfWork);

        var result = await handler.Handle(new LogoutCommand("refresh-token"), CancellationToken.None);

        Assert.Equal(MediatR.Unit.Value, result);
        Assert.True(token.IsRevoked);
        Assert.Equal(1, writeRepository.UpdateCallCount);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_WhenTokenDoesNotExist_DoesNotUpdateOrSave()
    {
        var readRepository = new FakeRefreshTokenReadRepository(null);
        var writeRepository = new FakeRefreshTokenWriteRepository();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new LogoutCommandHandler(readRepository, writeRepository, unitOfWork);

        var result = await handler.Handle(new LogoutCommand("missing-token"), CancellationToken.None);

        Assert.Equal(MediatR.Unit.Value, result);
        Assert.Equal(0, writeRepository.UpdateCallCount);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_WhenTokenAlreadyRevoked_DoesNotUpdateOrSave()
    {
        var token = RefreshToken.Create("refresh-token", "user-1", DateTime.UtcNow.AddDays(1));
        token.Revoke();
        var readRepository = new FakeRefreshTokenReadRepository(token);
        var writeRepository = new FakeRefreshTokenWriteRepository();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new LogoutCommandHandler(readRepository, writeRepository, unitOfWork);

        var result = await handler.Handle(new LogoutCommand("refresh-token"), CancellationToken.None);

        Assert.Equal(MediatR.Unit.Value, result);
        Assert.Equal(0, writeRepository.UpdateCallCount);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    private sealed class FakeRefreshTokenReadRepository(RefreshToken? refreshToken) : IRefreshTokenReadRepository
    {
        public Task<IReadOnlyList<RefreshToken>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<RefreshToken>>([]);

        public Task<RefreshToken?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<RefreshToken?>(null);

        public Task<IReadOnlyList<RefreshToken>> GetByIdsAsync(
            IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<RefreshToken>>([]);

        public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(false);

        public Task<int> CountAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(0);

        public Task<PagedResult<RefreshToken>> ListAsync(
            int page, int pageSize, CancellationToken cancellationToken = default)
            => Task.FromResult(new PagedResult<RefreshToken>([], 0, page, pageSize));

        public Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
            => Task.FromResult<RefreshToken?>(refreshToken);

        public Task<RefreshToken?> GetByTokenForUpdateAsync(string token, CancellationToken cancellationToken = default)
            => Task.FromResult<RefreshToken?>(refreshToken);
    }

    private sealed class FakeRefreshTokenWriteRepository : IRefreshTokenWriteRepository
    {
        public int UpdateCallCount { get; private set; }

        public Task AddAsync(RefreshToken entity, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task AddRangeAsync(IEnumerable<RefreshToken> entities, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public void Update(RefreshToken entity)
            => UpdateCallCount++;

        public void UpdateRange(IEnumerable<RefreshToken> entities)
        {
        }

        public void Remove(RefreshToken entity)
        {
        }

        public void RemoveRange(IEnumerable<RefreshToken> entities)
        {
        }
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
