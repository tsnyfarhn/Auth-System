using Api.Data;
using Api.DTOs.Authentication;
using Microsoft.EntityFrameworkCore;

namespace Api.Services.UserManagement;

public class UserService(AuthDbContext context)
{
    private readonly AuthDbContext _context = context;

    public async Task<(bool Success, string Message, CurrentUserResponse? Data)> GetCurrentUserAsync(Guid userId)
    {
        // --- Find user and retrieve roles and permissions ---
        var user = await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new CurrentUserResponse
            {
                Id = u.Id,
                Email = u.Email,
                EmailConfirmed = u.EmailConfirmed,
                IsActive = u.IsActive,

                Roles = u.UserRoles
                    .Select(ur => ur.Role.Name)
                    .Distinct()
                    .ToList(),

                Permissions = u.UserRoles
                    .SelectMany(ur => ur.Role.RolePermissions)
                    .Select(rp => rp.Permission.Code)
                    .Distinct()
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (user is null)
            return (false, "User not found.", null);

        if (!user.IsActive)
            return (false, "User account is inactive.", null);

        if (!user.EmailConfirmed)
            return (false, "Email has not been verified.", null);

        return (true, "User information retrieved successfull.", user);
    }
}