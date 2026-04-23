using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Dev4All.Application.Features.Auth.Commands.ForgotPassword;
using Dev4All.Application.Features.Auth.Commands.LoginUser;
using Dev4All.Application.Features.Auth.Commands.Logout;
using Dev4All.Application.Features.Auth.Commands.RefreshToken;
using Dev4All.Application.Features.Auth.Commands.RegisterUser;
using Dev4All.Application.Features.Auth.Common;
using Dev4All.Application.Features.Auth.Queries.GetCurrentUser;
using Dev4All.Domain.Enums;
using Dev4All.IntegrationTests.Infrastructure;

namespace Dev4All.IntegrationTests.Auth;

/// <summary>
/// End-to-end tests for <c>/api/v1/auth/*</c> endpoints covering registration,
/// login, current-user retrieval, refresh token rotation, logout, and forgot-password flows.
///
/// The factory is shared across all tests in this class (xUnit <see cref="IClassFixture{TFixture}"/>)
/// because <c>Quartz.Logging.LogProvider</c> caches the first <c>ILoggerFactory</c> globally and a
/// per-test factory would dispose it, breaking subsequent hosts. Tests guarantee isolation by using
/// a unique email/Guid per case.
/// </summary>
public sealed class AuthEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public AuthEndpointTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        factory.SeedRolesAsync().GetAwaiter().GetResult();
    }

    private const string StrongPassword = "Password1A!";

    [Fact]
    public async Task Register_WithValidRequest_ShouldReturn201()
    {
        var command = new RegisterUserCommand(
            "Alice Test",
            $"alice-{Guid.NewGuid():N}@test.com",
            StrongPassword,
            UserRole.Customer);

        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", command);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<RegisterUserResponse>();
        Assert.NotNull(body);
        Assert.Equal(command.Email, body!.Email);
        Assert.Equal(command.Name, body.Name);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ShouldReturn400()
    {
        var email = $"dup-{Guid.NewGuid():N}@test.com";
        var command = new RegisterUserCommand("Duplicate", email, StrongPassword, UserRole.Developer);

        var first = await _client.PostAsJsonAsync("/api/v1/auth/register", command);
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var second = await _client.PostAsJsonAsync("/api/v1/auth/register", command);

        Assert.Equal(HttpStatusCode.BadRequest, second.StatusCode);
    }

    [Fact]
    public async Task Register_WithInvalidPassword_ShouldReturn400()
    {
        var command = new RegisterUserCommand("Weak", $"weak-{Guid.NewGuid():N}@test.com", "weak", UserRole.Customer);

        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", command);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnAuthResponse()
    {
        var email = $"login-{Guid.NewGuid():N}@test.com";
        await RegisterAsync(email, StrongPassword, UserRole.Customer);

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new LoginUserCommand(email, StrongPassword));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(auth);
        Assert.False(string.IsNullOrWhiteSpace(auth!.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(auth.RefreshToken));
        Assert.Equal(email, auth.Email);
        Assert.Equal(nameof(UserRole.Customer), auth.Role);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturn401()
    {
        var email = $"badlogin-{Guid.NewGuid():N}@test.com";
        await RegisterAsync(email, StrongPassword, UserRole.Customer);

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new LoginUserCommand(email, "WrongPassword1!"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Me_WithValidToken_ShouldReturn200AndProfile()
    {
        var email = $"me-{Guid.NewGuid():N}@test.com";
        var auth = await RegisterAndLoginAsync(email, StrongPassword, UserRole.Developer);

        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/auth/me");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var profile = await response.Content.ReadFromJsonAsync<GetCurrentUserResponse>();
        Assert.NotNull(profile);
        Assert.Equal(email, profile!.Email);
        Assert.Equal(nameof(UserRole.Developer), profile.Role);
    }

    [Fact]
    public async Task Me_WithoutToken_ShouldReturn401()
    {
        var response = await _client.GetAsync("/api/v1/auth/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_WithValidPair_ShouldReturnNewPair()
    {
        var email = $"refresh-{Guid.NewGuid():N}@test.com";
        var auth = await RegisterAndLoginAsync(email, StrongPassword, UserRole.Customer);

        var response = await _client.PostAsJsonAsync("/api/v1/auth/refresh-token",
            new RefreshTokenCommand(auth.AccessToken, auth.RefreshToken));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var rotated = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(rotated);
        Assert.False(string.IsNullOrWhiteSpace(rotated!.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(rotated.RefreshToken));
        Assert.NotEqual(auth.RefreshToken, rotated.RefreshToken);
    }

    [Fact]
    public async Task Logout_WithValidRefreshToken_ShouldReturn204AndInvalidateToken()
    {
        var email = $"logout-{Guid.NewGuid():N}@test.com";
        var auth = await RegisterAndLoginAsync(email, StrongPassword, UserRole.Customer);

        var logout = await _client.PostAsJsonAsync("/api/v1/auth/logout",
            new LogoutCommand(auth.RefreshToken));

        Assert.Equal(HttpStatusCode.NoContent, logout.StatusCode);

        var reuseAttempt = await _client.PostAsJsonAsync("/api/v1/auth/refresh-token",
            new RefreshTokenCommand(auth.AccessToken, auth.RefreshToken));
        Assert.Equal(HttpStatusCode.Unauthorized, reuseAttempt.StatusCode);
    }

    [Fact]
    public async Task ForgotPassword_WithUnknownEmail_ShouldReturn202()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/auth/forgot-password",
            new ForgotPasswordCommand($"unknown-{Guid.NewGuid():N}@test.com"));

        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ForgotPasswordResponse>();
        Assert.NotNull(body);
    }

    private async Task RegisterAsync(string email, string password, UserRole role)
    {
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register",
            new RegisterUserCommand("Test User", email, password, role));
        response.EnsureSuccessStatusCode();
    }

    private async Task<AuthResponse> RegisterAndLoginAsync(string email, string password, UserRole role)
    {
        await RegisterAsync(email, password, role);
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new LoginUserCommand(email, password));
        loginResponse.EnsureSuccessStatusCode();
        var auth = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(auth);
        return auth!;
    }
}
