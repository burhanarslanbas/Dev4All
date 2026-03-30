namespace Dev4All.Application.Features.Auth.Commands.RegisterUser;

/// <summary>Result returned after successful user registration.</summary>
public sealed record RegisterUserResponse(Guid UserId, string Email, string Name);
