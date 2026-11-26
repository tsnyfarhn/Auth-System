namespace Api.Settings;

public class OtpSettings
{
    public string HmacSecret { get; set; } = string.Empty;
    public int Length { get; set; } = 6;
    public int ExpiryMinutes { get; set; } = 5;
    public int MaxAttempts { get; set; } = 5;
    public int ResendCooldownSeconds { get; set; } = 60;
}