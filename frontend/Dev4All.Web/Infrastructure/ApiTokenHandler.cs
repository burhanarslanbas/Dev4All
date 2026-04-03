using System.Net.Http.Headers;
using System.Security.Claims;

namespace Dev4All.Web.Infrastructure;

public sealed class ApiTokenHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    private static readonly string[] TokenClaimTypes = ["access_token", "token", "jwt"];

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.HttpContext?.User;

        if (user?.Identity?.IsAuthenticated == true)
        {
            var token = TokenClaimTypes
                .Select(user.FindFirstValue)
                .FirstOrDefault(static value => !string.IsNullOrWhiteSpace(value));

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        return base.SendAsync(request, cancellationToken);
    }
}
