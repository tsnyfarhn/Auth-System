using System.Security.Cryptography;
using System.Text;
using Api.Settings;

namespace Api.Services.Security;


public class OtpService(OtpSettings otpSettings)
{
    private readonly OtpSettings _otpSettings = otpSettings;

    public string GenerateOtp()
    {
        var maxValue = (int)Math.Pow(10, _otpSettings.Length);

        var otp = RandomNumberGenerator.GetInt32(0, maxValue);

        return otp.ToString($"D{_otpSettings.Length}");
    }

    public string GenerateOtpHash(Guid userId, string otp)
    {
        var key = Convert.FromBase64String(_otpSettings.HmacSecret);

        var data = $"{userId:N}:{otp}";

        var dataBytes = Encoding.UTF8.GetBytes(data);

        var hash = HMACSHA256.HashData(key, dataBytes);

        return Convert.ToHexString(hash);
    }

    public bool VerifyOtpHash(Guid userId, string otp, string storedHash)
    {
        var computedHash = GenerateOtpHash(userId, otp);

        var computedBytes = Convert.FromHexString(computedHash);
        var storedBytes = Convert.FromHexString(storedHash);

        return CryptographicOperations.FixedTimeEquals(computedBytes, storedBytes);
    }
}