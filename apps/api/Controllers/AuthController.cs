using Api.DTOs.Request.Authentication;
using Api.DTOs.Response.Authentication;
using Api.DTOs.Response.Common;
using Api.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(AuthService authService) : ControllerBase
{
    private readonly AuthService _authService = authService;

    [AllowAnonymous]
    [HttpPost("login")]
    [EnableRateLimiting("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        if (!result.Success)
            return Unauthorized(new ApiResponse<object>
            {
                Success = result.Success,
                Message = result.Message
            });

        return Ok(new ApiResponse<object>
        {
            Success = result.Success,
            Message = result.Message,
            Data = new LoginResponse
            {
                AccessToken = result.AccessToken!,
                RefreshToken = result.RefreshToken!
            }
        });
    }
}