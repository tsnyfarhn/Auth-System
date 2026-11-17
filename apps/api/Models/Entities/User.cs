using Api.Models.Base;

namespace Api.Models.Entities;

public class User : BaseEditableEntity
{
    public string Email { get; set; } = string.Empty;
    public string NormalizedEmail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public bool EmailConfirmed { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public int FailedLoginAttempts { get; set; } = 0;

    public DateTimeOffset? LastLoginAt { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = [];
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    public ICollection<ResetPasswordToken> ResetPasswordTokens { get; set; } = [];
}