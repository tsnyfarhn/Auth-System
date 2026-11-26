using System.Text.Encodings.Web;
using Api.Data;
using Api.DTOs.Request.Authentication;
using Api.Models.Entities;
using Api.Services.Auditing;
using Api.Services.Security;
using Api.Settings;
using Microsoft.EntityFrameworkCore;

namespace Api.Services.Authentication;

public class EmailVerificationService(AuthDbContext context, IConfiguration config, EmailService emailService, OtpService otpService, OtpSettings otpSettings, AuthLogService authLogService)
{
    private readonly AuthDbContext _context = context;
    private readonly IConfiguration _config = config;
    private readonly EmailService _emailService = emailService;
    private readonly OtpService _otpService = otpService;
    private readonly OtpSettings _otpSettings = otpSettings;
    private readonly AuthLogService _authLogService = authLogService;

    public async Task<(bool Success, string Message)> SendOtpEmailAsync(User user, bool checkResendCooldown = false)
    {
        // --- Generate OTP code and hashing ---
        var otp = _otpService.GenerateOtp();
        var otpHash = _otpService.GenerateOtpHash(user.Id, otp);

        // --- Create new email OTP  ---
        var newOtp = new EmailVerifyToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = otpHash,
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(_otpSettings.ExpiryMinutes),
            FailedAttempts = 0,
            CreatedAt = DateTimeOffset.UtcNow
        };

        // --- Begin transaction ---
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // --- Lock user transaction ---
            var lockedUser = await _context.Users
                .FromSqlInterpolated($"""
                    SELECT *
                    FROM auth.user
                    WHERE "Id" = {user.Id}
                    FOR UPDATE
                """)
                .AsTracking()
                .FirstOrDefaultAsync();

            if (lockedUser is null)
                return (false, "User not found.");

            if (lockedUser.EmailConfirmed)
                return (false, "Email has already been verified.");

            if (!lockedUser.IsActive)
                return (false, "User account is inactive.");

            // --- Check resend cooldown ---
            if (checkResendCooldown)
            {
                var isCooldownActive = await IsResendCooldownActiveAsync(lockedUser.Id);

                if (isCooldownActive)
                    return (false, $"Please wait {_otpSettings.ResendCooldownSeconds} seconds before requesting another OTP.");
            }

            // --- Get all old unused email OTP and revoke ---
            var oldOtps = await _context.EmailVerifyTokens
                .Where(eot =>
                    eot.UserId == user.Id &&
                    eot.UsedAt == null &&
                    eot.RevokedAt == null)
                .ToListAsync();

            foreach (var oldOtp in oldOtps)
            {
                oldOtp.RevokedAt = DateTimeOffset.UtcNow;
            }

            // --- Add new OTP ---
            _context.EmailVerifyTokens.Add(newOtp);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        // --- Create template email ---
        var htmlBody =
            $"""
                <h2>Verify Your Email Address</h2>

                <p>Hello {HtmlEncoder.Default.Encode(user.Email)},</p>

                <p>
                    Thank you for registering.
                    Your verification code is:
                </p>

                <h1>{otp}</h1>

                <p>This code will expire in {_otpSettings.ExpiryMinutes} minutes.</P>

                <p>If you did not create this account, you can safely ignore this email.</p>
            """;

        // --- Send email ---
        try
        {
            await _emailService.SendEmailAsync(
                recipient: user.Email,
                subject: "Verify Your Email Address",
                htmlBody: htmlBody
            );
        }
        catch
        {
            newOtp.RevokedAt = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync();

            throw;
        }

        return (true, "OTP email sent successfully.");
    }

    public async Task<(bool Success, string Message)> VerifyEmailAsync(VerifyEmailRequest request)
    {
        // --- Get user by email and validating ---
        var normalizedEmail = request.Email.Trim().ToUpperInvariant();

        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);

        if (user is null)
            return (false, "User not found.");

        if (user.EmailConfirmed)
            return (false, "Email has already been verified.");

        if (!user.IsActive)
            return (false, "User account is inactive.");

        // --- Update user status and OTP ---
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // --- Lock user transaction ---
            var lockedUser = await _context.Users
                .FromSqlInterpolated($"""
                    SELECT *
                    FROM auth.user
                    WHERE "Id" = {user.Id}
                    FOR UPDATE
                """)
                .AsTracking()
                .FirstOrDefaultAsync();

            if (lockedUser is null)
                return (false, "User not found.");

            if (lockedUser.EmailConfirmed)
                return (false, "Email has already been verified.");

            if (!lockedUser.IsActive)
                return (false, "User account is inactive.");

            // --- Get active OTP ---
            var activeOtp = await _context.EmailVerifyTokens
                .Where(eot =>
                    eot.UserId == lockedUser.Id &&
                    eot.UsedAt == null &&
                    eot.RevokedAt == null)
                .OrderByDescending(eot => eot.CreatedAt)
                .FirstOrDefaultAsync();

            if (activeOtp is null)
                return (false, "OTP code not found. Please request a new code.");

            // --- Check expires OTP ---
            if (activeOtp.ExpiresAt <= DateTimeOffset.UtcNow)
            {
                activeOtp.RevokedAt = DateTimeOffset.UtcNow;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // --- Create auth log ---
                await _authLogService.LogAsync(
                    lockedUser.Id,
                    "EMAIL_VERIFICATION_FAILED",
                    "User",
                    lockedUser.Id,
                    new
                    {
                        Reason = "OTP code expired."
                    }
                );

                return (false, "OTP code has expired. Please request a new code.");
            }

            // --- Check maximum attempts ---
            if (activeOtp.FailedAttempts >= _otpSettings.MaxAttempts)
            {
                activeOtp.RevokedAt = DateTimeOffset.UtcNow;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // --- Create auth log ---
                await _authLogService.LogAsync(
                    lockedUser.Id,
                    "EMAIL_VERIFICATION_FAILED",
                    "User",
                    lockedUser.Id,
                    new
                    {
                        Reason = "Maximum OTP attempts reached.",
                        FailedAttempts = activeOtp.FailedAttempts
                    }
                );

                return (false, "Maximum attempts reached. Please request a new code.");
            }

            var isValid = _otpService.VerifyOtpHash(lockedUser.Id, request.Otp, activeOtp.Token);

            if (!isValid)
            {
                activeOtp.FailedAttempts++;

                // --- Canceled OTP if maximum attempts reached ---
                if (activeOtp.FailedAttempts >= _otpSettings.MaxAttempts)
                {
                    activeOtp.RevokedAt = DateTimeOffset.UtcNow;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // --- Create auth log ---
                await _authLogService.LogAsync(
                    lockedUser.Id,
                    "EMAIL_VERIFICATION_FAILED",
                    "User",
                    lockedUser.Id,
                    new
                    {
                        Reason = "Invalid OTP code.",
                        FailedAttempts = activeOtp.FailedAttempts
                    }
                );

                if (activeOtp.RevokedAt is not null)
                    return (false, "Maximum attempts reached. Please request a new code.");

                return (false, "Invalid OTP code.");
            }

            activeOtp.UsedAt = DateTimeOffset.UtcNow;

            lockedUser.EmailConfirmed = true;
            lockedUser.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            // --- Create auth log ---
            await _authLogService.LogAsync(
                lockedUser.Id,
                "EMAIL_VERIFICATION_SUCCESS",
                "User",
                lockedUser.Id,
                new
                {
                    Reason = "User email successfully verified."
                }
            );

            return (true, "Email successfully verified.");
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<(bool Success, string Message)> ResendOtpEmailAsync(string email)
    {
        // --- Find user by email and validating ---
        var normalizedEmail = email.Trim().ToUpperInvariant();

        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);

        if (user is null)
            return (false, "User is not found.");

        if (user.EmailConfirmed)
            return (false, "Email of this user already confirmed.");

        if (!user.IsActive)
            return (false, "This user is inactive.");

        // --- Send email OTP ---
        var result = await SendOtpEmailAsync(user, checkResendCooldown: true);

        if (!result.Success)
            return result;

        // --- Create auth log ---
        await _authLogService.LogAsync(
            user.Id,
            "EMAIL_OTP_RESENT",
            "User",
            user.Id,
            new
            {
                Reason = "User requested a new email verification OTP."
            }
        );

        return (true, "Resend OTP successful.");
    }

    #region HELPER
    private async Task<bool> IsResendCooldownActiveAsync(Guid userId)
    {
        // --- Get last email OTP and validating ---
        var lastOtp = await _context.EmailVerifyTokens
            .Where(eot => eot.UserId == userId)
            .OrderByDescending(eot => eot.CreatedAt)
            .FirstOrDefaultAsync();

        if (lastOtp is null)
            return false;

        // --- Return when cooldown end ---
        var cooldownEnd = lastOtp.CreatedAt.AddSeconds(_otpSettings.ResendCooldownSeconds);

        return DateTimeOffset.UtcNow < cooldownEnd;
    }
    #endregion
}