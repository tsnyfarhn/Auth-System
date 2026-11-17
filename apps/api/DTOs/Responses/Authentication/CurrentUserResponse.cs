namespace Api.DTOs.Authentication;

public class CurrentUserResponse
{
    public Guid Id { get; set; }

    public string Email { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
    public bool IsActive { get; set; }

    public List<string> Roles { get; set; } = [];
    public List<string> Permissions { get; set; } = [];
}