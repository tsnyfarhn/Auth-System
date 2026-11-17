using System.Text.Json;
using Api.Data;
using Api.Models.Entities;

namespace Api.Services.Auditing;

public class AuthLogService(AuthDbContext context, IHttpContextAccessor httpContextAccessor)
{
    private readonly AuthDbContext _context = context;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task LogAsync(Guid? userId, string action, string? targetType = null, Guid? targetId = null, object? details = null)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        var authLog = new AuthLog
        {
            ActorUserId = userId,
            Action = action,
            TargetType = targetType,
            TargetId = targetId,
            Details = details == null ? null : JsonSerializer.Serialize(details),
            IpAddress = httpContext?.Connection.RemoteIpAddress?.ToString(),
            DeviceInfo = httpContext?.Request.Headers.UserAgent.ToString(),
            CreatedAt = DateTimeOffset.UtcNow
        };

        _context.AuthLogs.Add(authLog);

        await _context.SaveChangesAsync();
    }
}