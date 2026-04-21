namespace Dev4All.Web.Models.Auth;

public sealed record RegisterApiResponse(Guid UserId, string Email, string Name);
