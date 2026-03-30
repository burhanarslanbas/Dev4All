using MediatR;

namespace Dev4All.Application.Features.Auth.Queries.GetCurrentUser;

/// <summary>Returns the profile of the currently authenticated user.</summary>
public sealed record GetCurrentUserQuery() : IRequest<GetCurrentUserResponse>;
