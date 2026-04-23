using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Dev4All.Web.Models.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Dev4All.Web.Infrastructure;

public sealed class ApiTokenHandler(
    IHttpContextAccessor httpContextAccessor,
    IHttpClientFactory httpClientFactory) : DelegatingHandler
{
    private static readonly string[] TokenClaimTypes = ["access_token", "token", "jwt"];
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var context = httpContextAccessor.HttpContext;
        var user = context?.User;
        var accessToken = TokenClaimTypes
            .Select(t => user?.FindFirstValue(t))
            .FirstOrDefault(static v => !string.IsNullOrWhiteSpace(v));

        if (!string.IsNullOrWhiteSpace(accessToken))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized
            && context != null
            && user?.Identity?.IsAuthenticated == true
            && !string.IsNullOrWhiteSpace(accessToken))
        {
            var refreshToken = user.FindFirstValue("refresh_token");
            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                var newTokens = await TryRefreshAsync(accessToken, refreshToken, cancellationToken);
                if (newTokens != null)
                {
                    await UpdateAuthCookieAsync(context, user, newTokens);
                    using var retryRequest = await CloneRequestAsync(request, newTokens.AccessToken);
                    response.Dispose();
                    response = await base.SendAsync(retryRequest, cancellationToken);
                }
                else
                {
                    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                }
            }
        }

        return response;
    }

    private async Task<LoginApiResponse?> TryRefreshAsync(
        string accessToken, string refreshToken, CancellationToken ct)
    {
        try
        {
            var client = httpClientFactory.CreateClient("AuthService");
            var payload = JsonSerializer.Serialize(new { accessToken, refreshToken }, JsonOptions);
            using var content = new StringContent(payload, Encoding.UTF8, "application/json");
            using var response = await client.PostAsync("auth/refresh-token", content, ct);
            if (!response.IsSuccessStatusCode) return null;
            var body = await response.Content.ReadAsStringAsync(ct);
            return string.IsNullOrWhiteSpace(body)
                ? null
                : JsonSerializer.Deserialize<LoginApiResponse>(body, JsonOptions);
        }
        catch
        {
            return null;
        }
    }

    private static async Task UpdateAuthCookieAsync(
        HttpContext context, ClaimsPrincipal user, LoginApiResponse newTokens)
    {
        var updatedClaims = user.Claims
            .Where(c => c.Type != "access_token" && c.Type != "refresh_token")
            .Append(new Claim("access_token", newTokens.AccessToken))
            .Append(new Claim("refresh_token", newTokens.RefreshToken))
            .ToList();

        var identity = new ClaimsIdentity(updatedClaims, CookieAuthenticationDefaults.AuthenticationScheme);
        await context.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = true, ExpiresUtc = newTokens.ExpiresAt });
    }

    private static async Task<HttpRequestMessage> CloneRequestAsync(
        HttpRequestMessage original, string newAccessToken)
    {
        var clone = new HttpRequestMessage(original.Method, original.RequestUri);
        foreach (var (key, values) in original.Headers)
        {
            if (key.Equals("Authorization", StringComparison.OrdinalIgnoreCase)) continue;
            clone.Headers.TryAddWithoutValidation(key, values);
        }
        clone.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newAccessToken);
        if (original.Content != null)
        {
            var body = await original.Content.ReadAsStringAsync();
            clone.Content = new StringContent(body, Encoding.UTF8, "application/json");
        }
        return clone;
    }
}
