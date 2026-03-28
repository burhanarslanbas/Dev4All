using Dev4All.Domain.Exceptions.Base;

namespace Dev4All.Domain.Exceptions;

public class ResourceNotFoundException(string resource, Guid id)
    : DomainException($"{resource} with id '{id}' was not found.");
