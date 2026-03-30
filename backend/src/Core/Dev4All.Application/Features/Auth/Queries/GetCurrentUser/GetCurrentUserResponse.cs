namespace Dev4All.Application.Features.Auth.Queries.GetCurrentUser;

/// <summary>Profile information for the currently authenticated user.</summary>
public sealed record GetCurrentUserResponse(string UserId, string Email, string Role);
