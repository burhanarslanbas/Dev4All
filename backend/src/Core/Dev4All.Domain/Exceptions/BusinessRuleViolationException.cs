using Dev4All.Domain.Exceptions.Base;

namespace Dev4All.Domain.Exceptions;

public class BusinessRuleViolationException(string message) : DomainException(message);
