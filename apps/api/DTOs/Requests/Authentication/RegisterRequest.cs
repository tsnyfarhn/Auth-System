using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Request.Authentication;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    [RegularExpression(
        @"^(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9\s]).+$",
        ErrorMessage = "Password must contain at least one uppercase letter, one number, and one symbol."
    )]
    [MaxLength(255)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    [RegularExpression(
        @"^(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9\s]).+$",
        ErrorMessage = "Password must contain at least one uppercase letter, one number, and one symbol."
    )]
    [MaxLength(255)]
    [Compare(nameof(Password), ErrorMessage = "Password does not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}