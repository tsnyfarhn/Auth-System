using Api.Models.Base;

namespace Api.Models.Entities;

public class AuthLog : BaseEntity
{
    public Guid? ActorUserId { get; set; }
    public string Action { get; set; } = string.Empty;

    public string? TargetType { get; set; }
    public Guid? TargetId { get; set; }

    public string? Details { get; set; }
    public string? DeviceInfo { get; set; }
    public string? IpAddress { get; set; }
}