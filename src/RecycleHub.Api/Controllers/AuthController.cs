using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecycleHub.Api.Dtos.Requests;
using RecycleHub.Api.Dtos.Responses;
using RecycleHub.Api.Services.Interfaces;
using RecycleHub.Utils;

namespace RecycleHub.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ApiResponse<AuthResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<AuthResponse>))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ApiResponse<AuthResponse>))]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var response = await authService.RegisterAsync(request, HttpContext.RequestAborted);
        return StatusCode(response.Code, response);
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<AuthResponse>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiResponse<AuthResponse>))]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await authService.LoginAsync(request, HttpContext.RequestAborted);
        return StatusCode(response.Code, response);
    }

    [HttpPost("refresh-token")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<AuthResponse>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiResponse<AuthResponse>))]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var response = await authService.RefreshTokenAsync(request, HttpContext.RequestAborted);
        return StatusCode(response.Code, response);
    }

    [Authorize]
    [HttpGet("profile")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<UserProfileResponse>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<UserProfileResponse>))]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        var response = await authService.GetProfileAsync(userId, HttpContext.RequestAborted);
        return StatusCode(response.Code, response);
    }

    [Authorize]
    [HttpPut("profile")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<UserProfileResponse>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<UserProfileResponse>))]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        var response = await authService.UpdateProfileAsync(userId, request, HttpContext.RequestAborted);
        return StatusCode(response.Code, response);
    }
}
