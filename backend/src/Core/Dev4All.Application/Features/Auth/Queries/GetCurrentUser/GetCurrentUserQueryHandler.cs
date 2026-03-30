using Dev4All.Application.Abstractions.Auth;
using Dev4All.Domain.Exceptions;
using MediatR;

namespace Dev4All.Application.Features.Auth.Queries.GetCurrentUser;

/// <summary>Returns profile data for the current authenticated user.</summary>
public sealed class GetCurrentUserQueryHandler(
    ICurrentUser currentUser) : IRequestHandler<GetCurrentUserQuery, GetCurrentUserResponse>
{
    public Task<GetCurrentUserResponse> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated)
            throw new UnauthorizedDomainException("Kullanıcı doğrulaması gereklidir.");

        return Task.FromResult(
            new GetCurrentUserResponse(currentUser.UserId, currentUser.Email, currentUser.Role));
    }
}
