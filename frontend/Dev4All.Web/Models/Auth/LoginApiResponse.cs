namespace Dev4All.Web.Models.Auth;

public sealed record LoginApiResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    string Email,
    string Role);

public sealed record GetCurrentUserApiResponse(string UserId, string Email, string Role);
