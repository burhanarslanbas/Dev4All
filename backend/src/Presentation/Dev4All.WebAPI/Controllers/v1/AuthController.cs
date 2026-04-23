using Dev4All.Application.Features.Auth.Commands.ChangePassword;
using Dev4All.Application.Features.Auth.Commands.ConfirmEmail;
using Dev4All.Application.Features.Auth.Commands.ForgotPassword;
using Dev4All.Application.Features.Auth.Commands.LoginUser;
using Dev4All.Application.Features.Auth.Commands.Logout;
using Dev4All.Application.Features.Auth.Commands.RefreshToken;
using Dev4All.Application.Features.Auth.Commands.RegisterUser;
using Dev4All.Application.Features.Auth.Commands.ResendConfirmation;
using Dev4All.Application.Features.Auth.Commands.ResetPassword;
using Dev4All.Application.Features.Auth.Common;
using Dev4All.Application.Features.Auth.Queries.GetCurrentUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Dev4All.WebAPI.Controllers.v1;

/// <summary>Authentication and user account operations.</summary>
[ApiController]
[Route("api/v1/[controller]")]
public sealed class AuthController(ISender sender) : ControllerBase
{
    /// <summary>Registers a new user (Customer or Developer) and queues welcome + confirmation emails.</summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(typeof(RegisterUserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command, CancellationToken ct)
    {
        var response = await sender.Send(command, ct);
        return Created("/api/v1/auth/me", response);
    }

    /// <summary>Authenticates a user and returns an access + refresh token pair.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command, CancellationToken ct)
    {
        var response = await sender.Send(command, ct);
        return Ok(response);
    }

    /// <summary>Returns the profile of the current authenticated user.</summary>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(GetCurrentUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Me(CancellationToken ct)
    {
        var response = await sender.Send(new GetCurrentUserQuery(), ct);
        return Ok(response);
    }

    /// <summary>Exchanges an expired access token + valid refresh token for a new pair (rotation).</summary>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command, CancellationToken ct)
    {
        var response = await sender.Send(command, ct);
        return Ok(response);
    }

    /// <summary>Revokes the supplied refresh token (logout). Idempotent.</summary>
    [HttpPost("logout")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout([FromBody] LogoutCommand command, CancellationToken ct)
    {
        await sender.Send(command, ct);
        return NoContent();
    }

    /// <summary>Confirms a user's email using the supplied confirmation token.</summary>
    [HttpPost("confirm-email")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ConfirmEmailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailCommand command, CancellationToken ct)
    {
        var response = await sender.Send(command, ct);
        return Ok(response);
    }

    /// <summary>
    /// Initiates a password reset flow. Always returns 202 regardless of whether the email exists
    /// to prevent account enumeration.
    /// </summary>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(typeof(ForgotPasswordResponse), StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command, CancellationToken ct)
    {
        var response = await sender.Send(command, ct);
        return Accepted(response);
    }

    /// <summary>Resets a password using the token received by email.</summary>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(typeof(ResetPasswordResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command, CancellationToken ct)
    {
        var response = await sender.Send(command, ct);
        return Ok(response);
    }

    /// <summary>Changes the password for the current authenticated user.</summary>
    [Authorize]
    [HttpPost("change-password")]
    [ProducesResponseType(typeof(ChangePasswordResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command, CancellationToken ct)
    {
        var response = await sender.Send(command, ct);
        return Ok(response);
    }

    /// <summary>
    /// Re-sends the email confirmation link. Always returns 202 regardless of whether the email
    /// exists to prevent account enumeration.
    /// </summary>
    [HttpPost("resend-confirmation")]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(typeof(ResendConfirmationResponse), StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResendConfirmation([FromBody] ResendConfirmationCommand command, CancellationToken ct)
    {
        var response = await sender.Send(command, ct);
        return Accepted(response);
    }
}
