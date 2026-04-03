namespace Dev4All.Web.Models.Auth;

public sealed record LoginApiResponse(string Token, DateTime ExpiresAt, string Email, string Role);
