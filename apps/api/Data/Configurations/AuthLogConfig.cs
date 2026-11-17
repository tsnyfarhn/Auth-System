using Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Data.Configurations;

public class AuthLogConfig : IEntityTypeConfiguration<AuthLog>
{
    public void Configure(EntityTypeBuilder<AuthLog> builder)
    {
        // --- Table Name ---
        builder.ToTable("auth_log");

        // --- Primary Key ---
        builder.HasKey(al => al.Id);

        // --- Property ---
        builder.Property(al => al.Action)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(al => al.TargetType)
            .HasMaxLength(100);

        builder.Property(al => al.Details)
            .HasColumnType("jsonb");

        builder.Property(al => al.IpAddress)
            .HasMaxLength(45);

        builder.Property(al => al.DeviceInfo)
            .HasMaxLength(150);

        builder.Property(al => al.CreatedAt)
            .HasColumnType("timestamp with time zone");

        // --- Relation Table ---
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(al => al.ActorUserId)
            .OnDelete(DeleteBehavior.SetNull);

        // --- Index ---
        builder.HasIndex(al => al.CreatedAt)
            .HasDatabaseName("ix_auth_log_created_at");

        builder.HasIndex(al => al.ActorUserId)
            .HasDatabaseName("ix_auth_log_actor_user_id");

        builder.HasIndex(al => new { al.TargetType, al.TargetId })
            .HasDatabaseName("ix_auth_log_target_type_target_id");
    }
}