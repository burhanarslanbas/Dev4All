namespace Dev4All.Application.Features.Auth.Commands.Logout;

/// <summary>Result returned after processing a logout request.</summary>
public sealed record LogoutResponse(bool Success, string Message);
