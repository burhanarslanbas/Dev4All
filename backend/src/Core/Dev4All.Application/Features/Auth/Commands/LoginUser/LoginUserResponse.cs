namespace Dev4All.Application.Features.Auth.Commands.LoginUser;

/// <summary>Result returned after successful authentication.</summary>
public sealed record LoginUserResponse(string Token, DateTime ExpiresAt, string Email, string Role);
