using Dev4All.Application.Abstractions.Auth;
using Dev4All.Application.Features.Auth.Queries.GetCurrentUser;
using Dev4All.Domain.Exceptions;

namespace Dev4All.UnitTests.Features.Auth.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenAuthenticated_ShouldReturnProfile()
    {
        var currentUser = new FakeCurrentUser(
            userId: "user-1",
            email: "user1@example.com",
            role: "Customer",
            isAuthenticated: true);

        var handler = new GetCurrentUserQueryHandler(currentUser);

        var response = await handler.Handle(new GetCurrentUserQuery(), CancellationToken.None);

        Assert.Equal("user-1", response.UserId);
        Assert.Equal("user1@example.com", response.Email);
        Assert.Equal("Customer", response.Role);
    }

    [Fact]
    public async Task Handle_WhenNotAuthenticated_ShouldThrowAuthenticationFailedException()
    {
        var currentUser = new FakeCurrentUser(string.Empty, string.Empty, string.Empty, isAuthenticated: false);
        var handler = new GetCurrentUserQueryHandler(currentUser);

        await Assert.ThrowsAsync<AuthenticationFailedException>(() =>
            handler.Handle(new GetCurrentUserQuery(), CancellationToken.None));
    }

    private sealed class FakeCurrentUser(string userId, string email, string role, bool isAuthenticated) : ICurrentUser
    {
        public string UserId { get; } = userId;
        public string Email { get; } = email;
        public string Role { get; } = role;
        public bool IsAuthenticated { get; } = isAuthenticated;
    }
}
