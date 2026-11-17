using System.Security.Claims;
using Api.Services;
using Microsoft.AspNetCore.Authorization;

namespace Api.Authorization;

public class PermissionAuthorizationHandler(PermissionService permissionService) : AuthorizationHandler<PermissionRequirement>
{
    private readonly PermissionService _permissionService = permissionService;

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        // --- Get user ID from JWT claims ---
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
            return;

        // --- Get user permissions from database ---
        var permissions = await _permissionService.GetUserPermissionAsync(userId);

        // --- Validate user has the required permission ---
        if (permissions.Contains(requirement.PermissionCode))
            context.Succeed(requirement);
    }
}