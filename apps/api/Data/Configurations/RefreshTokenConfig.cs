using Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Data.Configurations;

public class RefreshTokenConfig : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        // --- Table Name ---
        builder.ToTable("refresh_token");

        // --- Primary Key ---
        builder.HasKey(rt => rt.Id);

        // --- Property ---
        builder.Property(rt => rt.Token)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(rt => rt.ExpiresAt)
            .HasColumnType("timestamp with time zone");

        builder.Property(rt => rt.CreatedAt)
            .HasColumnType("timestamp with time zone");

        builder.Property(rt => rt.RevokedAt)
            .HasColumnType("timestamp with time zone");

        builder.Property(rt => rt.CreatedByIp)
            .HasMaxLength(45);

        // --- Relation Table ---
        builder.HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<RefreshToken>()
            .WithMany()
            .HasForeignKey(rt => rt.ReplacedByTokenId)
            .OnDelete(DeleteBehavior.Restrict);

        // --- Unique Index ---
        builder.HasIndex(x => x.Token)
            .IsUnique()
            .HasDatabaseName("ux_refresh_token_hash");

        // --- Index ---
        builder.HasIndex(rt => rt.ReplacedByTokenId)
            .HasDatabaseName("ix_refresh_token_replace_by_token_id");

        builder.HasIndex(rt => new { rt.UserId, rt.ExpiresAt })
            .HasDatabaseName("ix_refresh_token_user_id_expires_at");
    }
}