namespace Dev4All.Application.Features.Auth.Common;

/// <summary>
/// Shared authentication response returned by login and refresh-token flows.
/// Contains the access token (short-lived JWT) and its matching refresh token
/// together with expiry metadata and the authenticated principal's summary.
/// </summary>
public sealed record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    string Email,
    string Role);
