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
    private static readonly HashSet<string> AllowedRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        "Customer",
        "Developer"
    };

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

    [HttpGet]
    [Route("auth/register")]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("auth/register")]
    public async Task<IActionResult> Register(RegisterViewModel model, CancellationToken ct)
    {
        if (!AllowedRoles.Contains(model.Role))
        {
            ModelState.AddModelError(nameof(model.Role), "Invalid role selection.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var response = await authService.RegisterAsync(model.Name, model.Email, model.Password, model.Role, ct);
            if (response is null)
            {
                ModelState.AddModelError(string.Empty, "Registration failed. Please try again.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Registration successful. Please login.";
            return RedirectToAction(nameof(Login));
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.BadRequest)
        {
            AddBackendValidationErrors(ex.Message);
            return View(model);
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(string.Empty, "Unable to connect to authentication service. Please try again.");
            return View(model);
        }
        catch (TaskCanceledException) when (!ct.IsCancellationRequested)
        {
            ModelState.AddModelError(string.Empty, "Registration request timed out. Please try again.");
            return View(model);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "An unexpected error occurred during registration.");
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

    private void AddBackendValidationErrors(string rawError)
    {
        try
        {
            using var json = JsonDocument.Parse(rawError);
            if (!json.RootElement.TryGetProperty("errors", out var errorsElement) || errorsElement.ValueKind != JsonValueKind.Array)
            {
                ModelState.AddModelError(string.Empty, "Validation failed. Please check your input and try again.");
                return;
            }

            var hasAnyError = false;
            foreach (var error in errorsElement.EnumerateArray())
            {
                if (!error.TryGetProperty("message", out var messageElement))
                {
                    continue;
                }

                var message = messageElement.GetString();
                if (string.IsNullOrWhiteSpace(message))
                {
                    continue;
                }

                var field = error.TryGetProperty("field", out var fieldElement)
                    ? fieldElement.GetString()
                    : null;

                ModelState.AddModelError(MapFieldName(field), message);
                hasAnyError = true;
            }

            if (!hasAnyError)
            {
                ModelState.AddModelError(string.Empty, "Validation failed. Please check your input and try again.");
            }
        }
        catch (JsonException)
        {
            ModelState.AddModelError(string.Empty, "Validation failed. Please check your input and try again.");
        }
    }

    private static string MapFieldName(string? fieldName) =>
        fieldName?.Trim().ToLowerInvariant() switch
        {
            "name" => nameof(RegisterViewModel.Name),
            "email" => nameof(RegisterViewModel.Email),
            "password" => nameof(RegisterViewModel.Password),
            "role" => nameof(RegisterViewModel.Role),
            _ => string.Empty
        };
}
