namespace Dev4All.Application.Common.Pagination;

/// <summary>Generic paginated result wrapper returned by list queries.</summary>
public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int Page,
    int PageSize);
