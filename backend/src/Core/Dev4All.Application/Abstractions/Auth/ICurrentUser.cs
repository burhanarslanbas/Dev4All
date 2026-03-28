namespace Dev4All.Application.Abstractions.Auth;

/// <summary>Provides identity information for the currently authenticated user.</summary>
public interface ICurrentUser
{
    string UserId { get; }
    string Email { get; }
    string Role { get; }
    bool IsAuthenticated { get; }
}
