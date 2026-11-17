using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Request.Authentication;

public class LoginRequest
{
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    [MaxLength(255)]
    public string Password { get; set; } = string.Empty;
}