using Dev4All.Web.Models.Auth;

namespace Dev4All.Web.Services;

public interface IAuthService
{
    Task<LoginApiResponse?> LoginAsync(string email, string password, CancellationToken ct = default);
    Task<RegisterApiResponse?> RegisterAsync(string name, string email, string password, string role, CancellationToken ct = default);
    Task<GetCurrentUserApiResponse?> GetCurrentUserAsync(string token, CancellationToken ct = default);
    Task<ConfirmEmailApiResponse?> ConfirmEmailAsync(string userId, string token, CancellationToken ct = default);
    Task<ResetPasswordApiResponse?> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken ct = default);
    Task LogoutAsync(string refreshToken, CancellationToken ct = default);
    Task<LoginApiResponse?> RefreshTokenAsync(string accessToken, string refreshToken, CancellationToken ct = default);
    Task ResendConfirmationAsync(string email, CancellationToken ct = default);
    Task ForgotPasswordAsync(string email, CancellationToken ct = default);
}
