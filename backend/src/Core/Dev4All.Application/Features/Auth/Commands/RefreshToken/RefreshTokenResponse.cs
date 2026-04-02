namespace Dev4All.Application.Features.Auth.Commands.RefreshToken;

public sealed record RefreshTokenResponse(string Token, string RefreshToken, DateTime ExpiresAt);
