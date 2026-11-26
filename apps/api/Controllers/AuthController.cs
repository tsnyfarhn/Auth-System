using Api.DTOs.Authentication;
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
public class AuthController(AuthService authService, EmailVerificationService emailVerificationService) : ControllerBase
{
    private readonly AuthService _authService = authService;
    private readonly EmailVerificationService _emailVerificationService = emailVerificationService;

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

    [AllowAnonymous]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshTokenRequest request)
    {
        var result = await _authService.LogoutAsync(request);

        if (!result.Success)
            return Unauthorized(new ApiResponse<object>
            {
                Success = result.Success,
                Message = result.Message
            });

        return Ok(new ApiResponse<object>
        {
            Success = result.Success,
            Message = result.Message
        });
    }

    [AllowAnonymous]
    [HttpPost("logout-all")]
    public async Task<IActionResult> LogoutAll(RefreshTokenRequest request)
    {
        var result = await _authService.LogoutAllAsync(request);

        if (!result.Success)
            return Unauthorized(new ApiResponse<object>
            {
                Success = result.Success,
                Message = result.Message
            });

        return Ok(new ApiResponse<object>
        {
            Success = result.Success,
            Message = result.Message
        });
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);

        if (!result.Success)
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = result.Message
            });

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = result.Message
        });
    }

    [AllowAnonymous]
    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail(VerifyEmailRequest request)
    {
        var result = await _emailVerificationService.VerifyEmailAsync(request);

        if (!result.Success)
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = result.Message
            });

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = result.Message
        });
    }

    [AllowAnonymous]
    [HttpPost("resend-verify")]
    public async Task<IActionResult> ResendVerifyEmail(ResendVerifyRequest request)
    {
        var result = await _emailVerificationService.ResendOtpEmailAsync(request.Email);

        if (!result.Success)
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = result.Message
            });

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = result.Message
        });
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
    {
        var result = await _authService.RefreshTokenAsync(request);

        if (!result.Success)
            return BadRequest(new ApiResponse<object>
            {
                Success = result.Success,
                Message = result.Message
            });

        return Ok(new ApiResponse<object>
        {
            Success = result.Success,
            Message = result.Message,
            Data = new TokenResponse
            {
                AccessToken = result.AccessToken!,
                RefreshToken = result.RefreshToken!
            }
        });
    }
}