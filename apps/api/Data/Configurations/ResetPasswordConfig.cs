using Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Data.Configurations;

public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<ResetPasswordToken>
{
    public void Configure(EntityTypeBuilder<ResetPasswordToken> builder)
    {
        // --- Table Name ---
        builder.ToTable("reset_password_token");

        // --- Primary Key ---
        builder.HasKey(rpt => rpt.Id);

        // --- Property ---
        builder.Property(rpt => rpt.Token)
            .IsRequired();

        builder.Property(rpt => rpt.ExpiresAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(rpt => rpt.UsedAt)
            .HasColumnType("timestamp with time zone");

        builder.Property(rpt => rpt.RevokedAt)
            .HasColumnType("timestamp with time zone");

        builder.Property(rpt => rpt.CreatedAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        // --- Relation Table ---
        builder.HasOne(rpt => rpt.User)
            .WithMany(u => u.ResetPasswordTokens)
            .HasForeignKey(rpt => rpt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // --- Index ---
        builder.HasIndex(rpt => rpt.UserId)
            .HasDatabaseName("ix_reset_password_token_user_id");

        // --- Unique Index ---
        builder.HasIndex(rpt => rpt.Token)
            .IsUnique()
            .HasDatabaseName("ux_reset_password_token_token");
    }
}