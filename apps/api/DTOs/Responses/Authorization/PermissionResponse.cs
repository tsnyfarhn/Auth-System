namespace Api.DTOs.Authorization;

public class PermissionResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

}