using Api.Data;
using Api.DTOs.Authentication;
using Api.DTOs.Request.Authentication;
using Api.Models.Entities;
using Api.Services.Auditing;
using Api.Services.Security;
using Api.Settings;
using Microsoft.EntityFrameworkCore;

namespace Api.Services.Authentication;

public class AuthService(AuthDbContext context, JwtService jwtService, JwtSettings jwtSettings, PasswordService passwordService, RefreshTokenService refreshTokenService, AuthLogService authLogService, IHttpContextAccessor httpContextAccessor, ILogger<AuthService> logger)
{
    private readonly AuthDbContext _context = context;
    private readonly JwtService _jwtService = jwtService;
    private readonly JwtSettings _jwtSettings = jwtSettings;
    private readonly PasswordService _passwordService = passwordService;
    private readonly RefreshTokenService _refreshTokenService = refreshTokenService;
    private readonly AuthLogService _authLogService = authLogService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ILogger<AuthService> _logger = logger;

    public async Task<(bool Success, string Message, string? AccessToken, string? RefreshToken)> LoginAsync(LoginRequest request)
    {
        // --- Request data ---
        var email = request.Email.Trim();
        var normalizedEmail = email.ToUpperInvariant();
        var password = request.Password;

        // --- Get user by email and validating ---
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);

        if (user is null)
        {
            // --- Create auth log ---
            await _authLogService.LogAsync(
                null,
                "LOGIN_FAILED",
                "User",
                null,
                new
                {
                    Message = "Account not found."
                }
            );

            return (false, "User not found.", null, null);
        }

        if (!user.EmailConfirmed)
        {
            // --- Create auth log ---
            await _authLogService.LogAsync(
                user.Id,
                "LOGIN_REJECTED",
                "User",
                user.Id,
                new
                {
                    Message = "Email has not been verified."
                }
            );

            return (false, "Email has not been verified.", null, null);
        }

        if (!user.IsActive)
        {
            // --- Create auth log ---
            await _authLogService.LogAsync(
                user.Id,
                "LOGIN_REJECTED",
                "User",
                user.Id,
                new
                {
                    Message = "User account is inactive."
                }
            );

            return (false, "User account is inactive.", null, null);
        }

        // --- Check account lock ---
        if (user.LockoutEnd is not null)
        {
            if (user.LockoutEnd > DateTimeOffset.UtcNow)
            {
                // --- Create auth log ---
                await _authLogService.LogAsync(
                    user.Id,
                    "LOGIN_BLOCKED",
                    "User",
                    user.Id,
                    new
                    {
                        Message = "Login attempted while account was locked."
                    }
                );

                return (false, "Account is temporarily locked. Please try again later.", null, null);
            }

            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;
        }

        // --- Validate password ---
        var isPasswordValid = _passwordService.VerifyPassword(password, user.Password);

        if (!isPasswordValid)
        {
            user.FailedLoginAttempts++;

            string action;
            object details;

            if (user.FailedLoginAttempts >= 5)
            {
                user.LockoutEnd = DateTimeOffset.UtcNow.AddMinutes(15);

                action = "ACCOUNT_LOCKED";
                details = new
                {
                    Message = "Accont locked after 5 failed login attempts.",
                    DurationMinutes = 15
                };
            }
            else
            {
                action = "LOGIN_FAILED";
                details = new
                {
                    Message = "Invalid password.",
                    FailedAttempts = user.FailedLoginAttempts
                };
            }

            user.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            // --- Create auth log ---
            await _authLogService.LogAsync(
                user.Id,
                action,
                "User",
                user.Id,
                details
            );

            if (user.LockoutEnd is not null)
                return (false, "Too many failed attempts. Account locked for 15 minutes.", null, null);

            return (false, "Invalid email or password.", null, null);
        }

        // --- Update last login and auth log ---
        user.FailedLoginAttempts = 0;
        user.LockoutEnd = null;
        user.LastLoginAt = DateTimeOffset.UtcNow;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        // --- Create auth log ---
        await _authLogService.LogAsync(
            user.Id,
            "LOGIN_SUCCESS",
            "User",
            user.Id,
            new
            {
                Message = "User logged in successfully."
            }
        );

        // --- Generate access token ---
        var accessToken = _jwtService.GenerateAccessToken(user);

        var refreshToken = _refreshTokenService.GenerateToken();
        var refreshTokenHash = _refreshTokenService.HashToken(refreshToken);

        // --- Save refresh token ---
        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenHash,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays),
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedByIp = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString()
        };

        _context.RefreshTokens.Add(refreshTokenEntity);

        await _context.SaveChangesAsync();

        return (true, "Login successful.", accessToken, refreshToken);
    }

    public async Task<(bool Success, string Message)> LogoutAsync(RefreshTokenRequest request)
    {
        var tokenHash = _refreshTokenService.HashToken(request.RefreshToken);

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // --- Find refresh token owner
            var tokenOwner = await _context.RefreshTokens
                .AsNoTracking()
                .Where(rt => rt.Token == tokenHash)
                .Select(rt => new { rt.UserId })
                .FirstOrDefaultAsync();

            if (tokenOwner is null)
                return (false, "Invalid refresh token.");

            // --- Lock and validate user ---
            var user = await _context.Users
                .FromSqlInterpolated($"""
                    SELECT *
                    FROM auth."user"
                    WHERE "Id" = {tokenOwner.UserId}
                    FOR UPDATE
                """)
                .FirstOrDefaultAsync();

            if (user is null)
                return (false, "User not found");

            // --- Lock and validate refresh token transaction ---
            var storedToken = await _context.RefreshTokens
                .FromSqlInterpolated($"""
                    SELECT * 
                    FROM auth.refresh_token
                    WHERE "Token" = {tokenHash}
                    FOR UPDATE
                """)
                .FirstOrDefaultAsync();

            if (storedToken is null)
                return (false, "Invalid refresh token.");

            if (storedToken.RevokedAt is not null)
                return (false, "Refresh token has alreafy been revoked.");

            if (storedToken.ExpiresAt <= DateTimeOffset.UtcNow)
                return (false, "Refresh token has expired.");

            // --- Revoke refresh token ---
            storedToken.RevokedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            // --- Create auth log ---
            await _authLogService.LogAsync(
                user.Id,
                "LOGOUT_SUCCESS",
                "User",
                user.Id,
                new
                {
                    Message = "User logged out successfully."
                }
            );

            return (true, "Logout successful.");
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync();

            // --- Logger ---
            _logger.LogError(
                exception,
                "Failed to log out user using a refresh token."
            );

            throw;
        }
    }

    public async Task<(bool Success, string Message)> LogoutAllAsync(RefreshTokenRequest request)
    {
        var tokenHash = _refreshTokenService.HashToken(request.RefreshToken);

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // --- Find refresh token owner
            var tokenOwner = await _context.RefreshTokens
                .AsNoTracking()
                .Where(rt => rt.Token == tokenHash)
                .Select(rt => new { rt.UserId })
                .FirstOrDefaultAsync();

            if (tokenOwner is null)
                return (false, "Invalid refresh token.");

            // --- Lock and validate user ---
            var user = await _context.Users
                .FromSqlInterpolated($"""
                    SELECT *
                    FROM auth."user"
                    WHERE "Id" = {tokenOwner.UserId}
                    FOR UPDATE
                """)
                .FirstOrDefaultAsync();

            if (user is null)
                return (false, "User not found");

            // --- Lock and validate refresh token transaction ---
            var storedToken = await _context.RefreshTokens
                .FromSqlInterpolated($"""
                    SELECT * 
                    FROM auth.refresh_token
                    WHERE "Token" = {tokenHash}
                    FOR UPDATE
                """)
                .FirstOrDefaultAsync();

            if (storedToken is null)
                return (false, "Invalid refresh token.");

            if (storedToken.RevokedAt is not null)
                return (false, "Refresh token has alreafy been revoked.");

            if (storedToken.ExpiresAt <= DateTimeOffset.UtcNow)
                return (false, "Refresh token has expired.");

            // --- Get and revoke all active refresh tokens ---
            var activeTokens = await _context.RefreshTokens
                .Where(rt =>
                    rt.UserId == storedToken.UserId &&
                    rt.RevokedAt == null &&
                    rt.ExpiresAt > DateTimeOffset.UtcNow)
                .ToListAsync();

            foreach (var token in activeTokens)
            {
                token.RevokedAt = DateTimeOffset.UtcNow;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            // --- Create auth log ---
            await _authLogService.LogAsync(
                user.Id,
                "LOGIN_ALL_SUCCESS",
                "User",
                user.Id,
                new
                {
                    Message = "User logged out from all devices successfully.",
                    RevokedSessions = activeTokens.Count
                }
            );

            return (true, "Logout from all devices successful.");
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync();

            // --- Logger ---
            _logger.LogError(
                exception,
                "Failed to log out user from all devices."
            );

            throw;
        }
    }
}