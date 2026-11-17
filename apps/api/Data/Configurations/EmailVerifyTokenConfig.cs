using Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Data.Configurations;

public class EmailVerifyTokenConfig : IEntityTypeConfiguration<EmailVerifyToken>
{
    public void Configure(EntityTypeBuilder<EmailVerifyToken> builder)
    {
        // --- Table Name ---
        builder.ToTable("email_verify_token");

        // --- Primary Key ---
        builder.HasKey(evt => evt.Id);

        // --- Property ---
        builder.Property(evt => evt.Token)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(evt => evt.FailedAttempts)
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(evt => evt.ExpiresAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(evt => evt.UsedAt)
            .HasColumnType("timestamp with time zone");

        builder.Property(evt => evt.RevokedAt)
            .HasColumnType("timestamp with time zone");

        builder.Property(evt => evt.CreatedAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        // --- Relation Table ---
        builder.HasOne(evt => evt.User)
            .WithMany()
            .HasForeignKey(evt => evt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // --- Index ---
        builder.HasIndex(evt => new { evt.UserId, evt.ExpiresAt })
            .HasDatabaseName("ix_email_verify_token_user_id_expires_at");

        builder.HasIndex(evt => new { evt.UserId, evt.Token })
            .HasDatabaseName("ix_email_verify_token_user_id_token");
    }
}