using Dev4All.Domain.Exceptions.Base;

namespace Dev4All.Domain.Exceptions;

public class UnauthorizedDomainException(string message) : DomainException(message);
