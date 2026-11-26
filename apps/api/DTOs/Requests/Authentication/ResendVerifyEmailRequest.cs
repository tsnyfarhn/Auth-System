using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Request.Authentication;

public class ResendVerifyRequest
{
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
}