using System.Net;
using System.Security.Claims;
using System.Text.Json;
using Dev4All.Web.Models.Auth;
using Dev4All.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Dev4All.Web.Controllers;

public sealed class AuthController(IAuthService authService) : Controller
{
    [HttpGet]
    [Route("auth/login")]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("auth/login")]
    public async Task<IActionResult> Login(LoginViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var response = await authService.LoginAsync(model.Email, model.Password, ct);
            if (response is null)
            {
                ModelState.AddModelError(string.Empty, "Login failed. Please try again.");
                return View(model);
            }

            var tokenUserId = GetUserIdFromToken(response.Token);
            if (string.IsNullOrWhiteSpace(tokenUserId))
            {
                ModelState.AddModelError(string.Empty, "Invalid login token.");
                return View(model);
            }

            var currentUser = await authService.GetCurrentUserAsync(response.Token, ct);
            if (currentUser is null || string.IsNullOrWhiteSpace(currentUser.UserId))
            {
                ModelState.AddModelError(string.Empty, "Unable to load user profile.");
                return View(model);
            }
            if (!string.Equals(tokenUserId, currentUser.UserId, StringComparison.Ordinal))
            {
                ModelState.AddModelError(string.Empty, "Unable to validate login token.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, currentUser.UserId),
                new(ClaimTypes.Name, currentUser.Email),
                new(ClaimTypes.Email, currentUser.Email),
                new(ClaimTypes.Role, currentUser.Role),
                new("access_token", response.Token)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = response.ExpiresAt
                });

            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(model);
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(string.Empty, "Unable to connect to authentication service. Please try again.");
            return View(model);
        }
        catch (TaskCanceledException) when (!ct.IsCancellationRequested)
        {
            ModelState.AddModelError(string.Empty, "Login request timed out. Please try again.");
            return View(model);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "An unexpected error occurred during login.");
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("auth/logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    private static string? GetUserIdFromToken(string token)
    {
        var segments = token.Split('.');
        if (segments.Length < 2)
        {
            return null;
        }

        try
        {
            var payloadBytes = WebEncoders.Base64UrlDecode(segments[1]);
            using var json = JsonDocument.Parse(payloadBytes);
            return json.RootElement.TryGetProperty("sub", out var subClaim)
                ? subClaim.GetString()
                : null;
        }
        catch (FormatException)
        {
            return null;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
