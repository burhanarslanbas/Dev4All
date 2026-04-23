using Dev4All.Domain.Exceptions.Base;

namespace Dev4All.Domain.Exceptions;

/// <summary>
/// Raised when authentication (identity verification) fails.
/// Mapped to HTTP 401 by the global exception middleware.
/// Distinct from <see cref="UnauthorizedDomainException"/>, which represents
/// an authorization (forbidden) failure and maps to HTTP 403.
/// </summary>
public class AuthenticationFailedException(string message) : DomainException(message);
