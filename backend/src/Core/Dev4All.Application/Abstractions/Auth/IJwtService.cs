using System.Security.Claims;

namespace Dev4All.Application.Abstractions.Auth;

/// <summary>Abstraction for JWT token generation. Uses primitive parameters to avoid Persistence coupling.</summary>
public interface IJwtService
{
    /// <summary>Generates a signed JWT token for the given user identity.</summary>
    string GenerateToken(string userId, string email, string role);

    string GenerateRefreshToken();

    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
