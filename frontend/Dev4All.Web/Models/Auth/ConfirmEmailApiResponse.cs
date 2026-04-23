namespace Dev4All.Web.Models.Auth;

public sealed record ConfirmEmailApiResponse(bool Success, string Message);

public sealed record VerifyEmailViewModel(bool IsSuccess, string Message);
