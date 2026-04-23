using Dev4All.Web.Models.Auth;
using System.Net.Http.Headers;

namespace Dev4All.Web.Services;

public sealed class AuthService(IApiClient apiClient, IHttpClientFactory httpClientFactory) : IAuthService
{
    private const int CustomerRole = 0;
    private const int DeveloperRole = 1;

    public Task<LoginApiResponse?> LoginAsync(string email, string password, CancellationToken ct = default) =>
        apiClient.PostAsync<LoginApiResponse>(
            "auth/login",
            new { email, password },
            ct);

    public Task<RegisterApiResponse?> RegisterAsync(
        string name,
        string email,
        string password,
        string role,
        CancellationToken ct = default)
    {
        var roleValue = ParseRole(role);

        return apiClient.PostAsync<RegisterApiResponse>(
            "auth/register",
            new
            {
                name,
                email,
                password,
                role = roleValue
            },
            ct);
    }

    public async Task<GetCurrentUserApiResponse?> GetCurrentUserAsync(string token, CancellationToken ct = default)
    {
        var client = httpClientFactory.CreateClient(nameof(AuthService));
        if (client.BaseAddress is null)
        {
            throw new InvalidOperationException("AuthService HttpClient base address is not configured.");
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, "auth/me");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await client.SendAsync(request, ct);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Unable to read current user. Status code: {(int)response.StatusCode}.",
                null,
                response.StatusCode);
        }

        return await response.Content.ReadFromJsonAsync<GetCurrentUserApiResponse>(cancellationToken: ct);
    }

    public Task<ConfirmEmailApiResponse?> ConfirmEmailAsync(string userId, string token, CancellationToken ct = default) =>
        apiClient.PostAsync<ConfirmEmailApiResponse>(
            "auth/confirm-email",
            new
            {
                userId,
                token
            },
            ct);

    public Task<ResetPasswordApiResponse?> ResetPasswordAsync(
        string email,
        string token,
        string newPassword,
        CancellationToken ct = default) =>
        apiClient.PostAsync<ResetPasswordApiResponse>(
            "auth/reset-password",
            new
            {
                email,
                token,
                newPassword
            },
            ct);

    public async Task LogoutAsync(string refreshToken, CancellationToken ct = default)
    {
        try
        {
            await apiClient.PostAsync<object>("auth/logout", new { refreshToken }, ct);
        }
        catch
        {
            // Logout best-effort: local session cleared regardless of backend response
        }
    }

    public Task<LoginApiResponse?> RefreshTokenAsync(string accessToken, string refreshToken, CancellationToken ct = default) =>
        apiClient.PostAsync<LoginApiResponse>(
            "auth/refresh-token",
            new { accessToken, refreshToken },
            ct);

    public async Task ResendConfirmationAsync(string email, CancellationToken ct = default)
    {
        await apiClient.PostAsync<object>("auth/resend-confirmation", new { email }, ct);
    }

    public async Task ForgotPasswordAsync(string email, CancellationToken ct = default)
    {
        await apiClient.PostAsync<object>("auth/forgot-password", new { email }, ct);
    }

    private static int ParseRole(string role) => role.Trim().ToLowerInvariant() switch
    {
        "developer" => DeveloperRole,
        _ => CustomerRole
    };
}
