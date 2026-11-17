using Api.Data;
using Api.DTOs.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public class PermissionService
{
    private readonly AuthDbContext _context;

    public PermissionService(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<List<PermissionResponse>> GetAllPermissionAsync()
    {
        var permissions = await _context.Permissions
            .AsNoTracking()
            .OrderBy(p => p.Module)
            .ThenBy(p => p.Name)
            .Select(p => new PermissionResponse
            {
                Id = p.Id,
                Name = p.Name,
                Module = p.Module,
                Code = p.Code,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .ToListAsync();

        return permissions;
    }


    public async Task<List<string>> GetUserPermissionAsync(Guid userId)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Code)
            .Distinct()
            .ToListAsync();
    }
}