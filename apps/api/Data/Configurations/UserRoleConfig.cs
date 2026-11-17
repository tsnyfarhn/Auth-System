using Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Data.Configurations;

public class UserRoleConfig : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        // --- Table Name ---
        builder.ToTable("user_role");

        // --- Composite Primary Key ---
        builder.HasKey(ur => new { ur.UserId, ur.RoleId });

        // --- Property ---
        builder.Property(ur => ur.AssignedAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        // --- Relation Table ---
        builder.HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(ur => ur.AssignedBy)
            .OnDelete(DeleteBehavior.SetNull);

        // --- Index ---
        builder.HasIndex(ur => ur.RoleId)
            .HasDatabaseName("ix_user_role_role_id");

        builder.HasIndex(ur => ur.AssignedBy)
            .HasDatabaseName("ix_user_role_assigned_by");

        // --- Initial Data (Admin) ---
        builder.HasData(
            new UserRole
            {
                UserId = Guid.Parse("cc42e7a1-b515-4e29-97b9-991c1df22365"),
                RoleId = Guid.Parse("049072c7-a124-41b4-bb13-b47454934030"),
                AssignedBy = Guid.Parse("cc42e7a1-b515-4e29-97b9-991c1df22365"),
                AssignedAt = new DateTimeOffset(2026, 10, 1, 0, 0, 0, TimeSpan.Zero)
            }
        );
    }
}