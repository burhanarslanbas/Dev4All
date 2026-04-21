namespace Dev4All.Web.Models.Auth;

public sealed record LoginApiResponse(string Token, DateTime ExpiresAt, string Email, string Role);

public sealed record GetCurrentUserApiResponse(string UserId, string Email, string Role);
