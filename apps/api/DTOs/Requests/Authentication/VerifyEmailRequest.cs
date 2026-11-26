using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Request.Authentication;

public class VerifyEmailRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP must contain exactly 6 digits")]
    public string Otp { get; set; } = string.Empty;
}