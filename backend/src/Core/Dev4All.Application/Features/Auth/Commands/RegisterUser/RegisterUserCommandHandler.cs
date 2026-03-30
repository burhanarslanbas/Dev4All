using Dev4All.Application.Abstractions.Auth;
using Dev4All.Domain.Enums;
using Dev4All.Domain.Exceptions;
using MediatR;

namespace Dev4All.Application.Features.Auth.Commands.RegisterUser;

/// <summary>Handles user registration using the identity abstraction.</summary>
public sealed class RegisterUserCommandHandler(
    IIdentityService identityService) : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
{
    public async Task<RegisterUserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (request.Role == UserRole.Admin)
            throw new UnauthorizedDomainException("Admin rolü public register endpoint'i üzerinden oluşturulamaz.");

        var (succeeded, userId, errors) = await identityService.CreateUserAsync(
            request.Name,
            request.Email,
            request.Password,
            request.Role.ToString(),
            cancellationToken);

        if (!succeeded)
            throw new BusinessRuleViolationException(string.Join(", ", errors));

        return new RegisterUserResponse(Guid.Parse(userId), request.Email, request.Name);
    }
}
