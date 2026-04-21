using Dev4All.Domain.Common;
using Dev4All.Domain.Exceptions;

namespace Dev4All.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public string Token { get; private set; } = string.Empty;
    public string UserId { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    public bool IsActive => !IsRevoked && ExpiresAt > DateTime.UtcNow;

    public static RefreshToken Create(string token, string userId, DateTime expiresAt)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new BusinessRuleViolationException("Refresh token cannot be empty.");
        if (string.IsNullOrWhiteSpace(userId))
            throw new BusinessRuleViolationException("User ID cannot be empty.");

        return new RefreshToken
        {
            Token = token,
            UserId = userId,
            ExpiresAt = expiresAt
        };
    }

    public void Revoke()
    {
        if (IsRevoked)
            return;

        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
        MarkAsUpdated();
    }
}
