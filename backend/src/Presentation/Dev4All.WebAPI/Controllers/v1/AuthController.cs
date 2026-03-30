using Dev4All.Application.Features.Auth.Commands.LoginUser;
using Dev4All.Application.Features.Auth.Commands.RegisterUser;
using Dev4All.Application.Features.Auth.Queries.GetCurrentUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dev4All.WebAPI.Controllers.v1;

/// <summary>Authentication and user operations endpoints.</summary>
[ApiController]
[Route("api/v1/[controller]")]
public sealed class AuthController(ISender sender) : ControllerBase
{
    /// <summary>Registers a new user.</summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterUserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command, CancellationToken ct)
    {
        var response = await sender.Send(command, ct);
        return Created("/api/v1/auth/me", response);
    }

    /// <summary>Logs in a user and returns a JWT token response.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command, CancellationToken ct)
    {
        var response = await sender.Send(command, ct);
        return Ok(response);
    }

    /// <summary>Returns profile of the current authenticated user.</summary>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(GetCurrentUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Me(CancellationToken ct)
    {
        var response = await sender.Send(new GetCurrentUserQuery(), ct);
        return Ok(response);
    }
}
