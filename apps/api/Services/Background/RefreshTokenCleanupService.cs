using Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Services.Background;

public class RefreshTokenCleanupService(IServiceScopeFactory serviceScopeFactory, ILogger<RefreshTokenCleanupService> logger) : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    private readonly ILogger<RefreshTokenCleanupService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(5));

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await RevokeExpiredTokensAsync(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to revoke expired refresh tokens.");
            }

            try
            {
                await timer.WaitForNextTickAsync(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
        }
    }

    protected async Task RevokeExpiredTokensAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

        var expiredTokens = await context.RefreshTokens
            .Where(token =>
                token.RevokedAt == null &&
                token.ExpiresAt <= DateTimeOffset.UtcNow)
            .ToListAsync(cancellationToken);

        foreach (var token in expiredTokens)
        {
            token.RevokedAt = DateTimeOffset.UtcNow;
        }

        if (expiredTokens.Count > 0)
        {
            await context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Revoked {TokenCount} expired refresh tokens.",
                expiredTokens.Count
            );
        }
    }
}