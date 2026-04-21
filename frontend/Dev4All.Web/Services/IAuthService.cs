using Dev4All.Web.Models.Auth;

namespace Dev4All.Web.Services;

public interface IAuthService
{
    Task<LoginApiResponse?> LoginAsync(string email, string password, CancellationToken ct = default);
    Task<RegisterApiResponse?> RegisterAsync(string name, string email, string password, string role, CancellationToken ct = default);
    Task<GetCurrentUserApiResponse?> GetCurrentUserAsync(string token, CancellationToken ct = default);
}
