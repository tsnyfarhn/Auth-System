using System.Security.Claims;
using Api.DTOs.Response.Common;
using Api.Services.UserManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/users")]
public class UserController(UserService userService) : ControllerBase
{
    private readonly UserService _userService = userService;

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        // --- Get and validate user permission ---
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new ApiResponse<object>
            {
                Success = false,
                Message = "Invalid user identity"
            });
        }

        var result = await _userService.GetCurrentUserAsync(userId);

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
            Data = result.Data
        });
    }
}