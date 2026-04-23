using System.Net;
using System.Security.Claims;
using System.Text.Json;
using Dev4All.Web.Infrastructure;
using Dev4All.Web.Models.Auth;
using Dev4All.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;

namespace Dev4All.Web.Controllers;

[Route("[controller]")]
public sealed class AuthController(IAuthService authService) : Controller
{
    private static readonly HashSet<string> AllowedRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        "Customer",
        "Developer"
    };

    [HttpGet("login")]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost("login")]
    [ValidateAntiForgeryToken]
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
                ModelState.AddModelError(string.Empty, "Giriş başarısız. Lütfen tekrar deneyin.");
                return View(model);
            }

            var tokenUserId = GetUserIdFromToken(response.AccessToken);
            if (string.IsNullOrWhiteSpace(tokenUserId))
            {
                ModelState.AddModelError(string.Empty, "Giriş belirteci geçersiz.");
                return View(model);
            }

            var currentUser = await authService.GetCurrentUserAsync(response.AccessToken, ct);
            if (currentUser is null || string.IsNullOrWhiteSpace(currentUser.UserId))
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı profili yüklenemedi.");
                return View(model);
            }
            if (!string.Equals(tokenUserId, currentUser.UserId, StringComparison.Ordinal))
            {
                ModelState.AddModelError(string.Empty, "Giriş belirteci doğrulanamadı.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, currentUser.UserId),
                new(ClaimTypes.Name, currentUser.Email),
                new(ClaimTypes.Email, currentUser.Email),
                new(ClaimTypes.Role, currentUser.Role),
                new("access_token", response.AccessToken),
                new("refresh_token", response.RefreshToken)
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

            if (currentUser.Role.Equals(AppRoles.Developer, StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Explore", "Projects");
            }

            return RedirectToAction("Index", "Projects");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            ModelState.AddModelError(string.Empty, "E-posta veya şifre hatalı.");
            return View(model);
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(string.Empty, "Kimlik doğrulama servisine bağlanılamadı. Lütfen tekrar deneyin.");
            return View(model);
        }
        catch (TaskCanceledException) when (!ct.IsCancellationRequested)
        {
            ModelState.AddModelError(string.Empty, "Giriş isteği zaman aşımına uğradı. Lütfen tekrar deneyin.");
            return View(model);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "Giriş sırasında beklenmeyen bir hata oluştu.");
            return View(model);
        }
    }

    [HttpGet("register")]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new RegisterViewModel());
    }

    [HttpPost("register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, CancellationToken ct)
    {
        if (!AllowedRoles.Contains(model.Role))
        {
            ModelState.AddModelError(nameof(model.Role), "Geçersiz rol seçimi.");
        }

        if (!model.TermsAccepted)
        {
            ModelState.AddModelError(nameof(model.TermsAccepted), "Kullanım koşullarını kabul etmelisiniz.");
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
                ModelState.AddModelError(string.Empty, "Kayıt servisi beklenmeyen bir yanıt döndürdü. Lütfen daha sonra tekrar deneyin.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Kayıt başarılı! E-postanıza doğrulama linki gönderildi.";
            TempData["PendingConfirmationEmail"] = model.Email;
            return RedirectToAction(nameof(Login));
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.BadRequest)
        {
            AddBackendValidationErrors(ex.Message);
            return View(model);
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(string.Empty, "Kimlik doğrulama servisine bağlanılamadı. Lütfen tekrar deneyin.");
            return View(model);
        }
        catch (TaskCanceledException) when (!ct.IsCancellationRequested)
        {
            ModelState.AddModelError(string.Empty, "Kayıt isteği zaman aşımına uğradı. Lütfen tekrar deneyin.");
            return View(model);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "Kayıt sırasında beklenmeyen bir hata oluştu.");
            return View(model);
        }
    }

    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var refreshToken = User.FindFirstValue("refresh_token");
        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            await authService.LogoutAsync(refreshToken, ct);
        }

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet("forgot-password")]
    public IActionResult ForgotPassword()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        return View(new ForgotPasswordViewModel());
    }

    [HttpPost("forgot-password")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await authService.ForgotPasswordAsync(model.Email, ct);
        }
        catch
        {
            // Account enumeration protection: always show success regardless of backend result
        }

        model.Submitted = true;
        return View(model);
    }

    [HttpPost("resend-confirmation")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResendConfirmation([FromForm] string email, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(email))
        {
            try
            {
                await authService.ResendConfirmationAsync(email, ct);
            }
            catch
            {
                // Account enumeration protection: silently succeed
            }
        }

        TempData["SuccessMessage"] = "Doğrulama e-postası tekrar gönderildi. Lütfen gelen kutunuzu kontrol edin.";
        TempData["PendingConfirmationEmail"] = email;
        return RedirectToAction(nameof(Login));
    }

    [HttpGet("access-denied")]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail(string? userId, string? token, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
        {
            return View(new VerifyEmailViewModel(false, "Doğrulama bağlantısı geçersiz veya eksik."));
        }

        try
        {
            var response = await authService.ConfirmEmailAsync(userId, token, ct);
            if (response is null)
            {
                return View(new VerifyEmailViewModel(false, "Doğrulama servisi beklenmeyen bir yanıt döndürdü."));
            }

            return View(new VerifyEmailViewModel(response.Success, response.Message));
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.BadRequest)
        {
            return View(new VerifyEmailViewModel(false, "Doğrulama bağlantısı geçersiz veya süresi dolmuş."));
        }
        catch (TaskCanceledException) when (!ct.IsCancellationRequested)
        {
            return View(new VerifyEmailViewModel(false, "Doğrulama isteği zaman aşımına uğradı. Lütfen tekrar deneyin."));
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception)
        {
            return View(new VerifyEmailViewModel(false, "E-posta doğrulama sırasında beklenmeyen bir hata oluştu."));
        }
    }

    [HttpGet("reset-password")]
    public IActionResult ResetPassword(string? email, string? token)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
        {
            return View(new ResetPasswordViewModel
            {
                Completed = true,
                ResultMessage = "Şifre sıfırlama bağlantısı geçersiz veya eksik."
            });
        }

        return View(new ResetPasswordViewModel
        {
            Email = email,
            Token = token
        });
    }

    [HttpPost("reset-password")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var response = await authService.ResetPasswordAsync(model.Email, model.Token, model.NewPassword, ct);
            if (response is null)
            {
                model.Completed = true;
                model.ResultMessage = "Şifre sıfırlama servisi beklenmeyen bir sonuç döndürdü.";
                return View(model);
            }

            model.Completed = response.Success;
            model.ResultMessage = response.Message;
            return View(model);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.BadRequest)
        {
            model.Completed = true;
            model.ResultMessage = "Şifre sıfırlama bağlantısı geçersiz veya süresi dolmuş olabilir.";
            return View(model);
        }
        catch (TaskCanceledException) when (!ct.IsCancellationRequested)
        {
            model.Completed = true;
            model.ResultMessage = "Şifre sıfırlama isteği zaman aşımına uğradı. Lütfen tekrar deneyin.";
            return View(model);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception)
        {
            model.Completed = true;
            model.ResultMessage = "Beklenmeyen bir hata oluştu. Lütfen daha sonra tekrar deneyin.";
            return View(model);
        }
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
                ModelState.AddModelError(string.Empty, "Doğrulama başarısız. Lütfen girdilerinizi kontrol edin.");
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
                ModelState.AddModelError(string.Empty, "Doğrulama başarısız. Lütfen girdilerinizi kontrol edin.");
            }
        }
        catch (JsonException)
        {
            ModelState.AddModelError(string.Empty, "Doğrulama başarısız. Lütfen girdilerinizi kontrol edin.");
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
