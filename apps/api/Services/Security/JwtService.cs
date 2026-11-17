using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Api.Models.Entities;
using Api.Settings;
using Microsoft.IdentityModel.Tokens;

namespace Api.Services.Security;

public class JwtService(JwtSettings jwtSettings)
{
    private readonly JwtSettings _jwtSettings = jwtSettings;

    public string GenerateAccessToken(User user)
    {
        // --- Create signing key ---
        var secretKey = Convert.FromBase64String(_jwtSettings.SecretKey);
        var securityKey = new SymmetricSecurityKey(secretKey);

        // --- Create signing credentials ---
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // --- Create claims ---
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // --- Generate JWT ---
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}