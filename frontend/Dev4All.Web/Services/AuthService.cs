using Dev4All.Web.Models.Auth;

namespace Dev4All.Web.Services;

public sealed class AuthService(IApiClient apiClient) : IAuthService
{
    public Task<LoginApiResponse?> LoginAsync(string email, string password, CancellationToken ct = default) =>
        apiClient.PostAsync<LoginApiResponse>(
            "auth/login",
            new
            {
                email,
                password
            },
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

    private static int ParseRole(string role) => role.Trim().ToLowerInvariant() switch
    {
        "developer" => 1,
        _ => 0
    };
}
