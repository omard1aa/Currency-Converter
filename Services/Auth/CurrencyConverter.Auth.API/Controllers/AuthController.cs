using CurrencyConverter.Auth.Application.DTOs;
using CurrencyConverter.Auth.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverter.Auth.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var response = await _authService.RegisterAsync(request);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userId = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
            return Unauthorized();

        await _authService.LogoutAsync(parsedUserId);
        return Ok(new { message = "Logged out successfully" });
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var response = await _authService.RefreshTokenAsync(request.RefreshToken);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; }
}