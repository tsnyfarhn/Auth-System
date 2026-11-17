using Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Data.Configurations;

public class PermissionConfig : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        // --- Table Name ---
        builder.ToTable("permission");

        // --- Primary Key ---
        builder.HasKey(p => p.Id);

        // --- Property ---
        builder.Property(p => p.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(p => p.Module)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Code)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.CreatedAt)
            .HasColumnType("timestamp with time zone");

        builder.Property(p => p.UpdatedAt)
            .HasColumnType("timestamp with time zone");

        // --- Relation Table ---
        builder.HasMany(p => p.RolePermissions)
            .WithOne(rp => rp.Permission)
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);

        // --- Unique Index ---
        builder.HasIndex(p => p.Code)
            .IsUnique()
            .HasDatabaseName("ux_permission_code");

        // --- Initial Data ---
        builder.HasData(
            new Permission
            {
                Id = Guid.Parse("9f028d8a-b926-4799-9975-9d2cdfc92a1d"),
                Name = "View Dashboard",
                Module = "Dashboard",
                Code = "dashboard.read",
                CreatedAt = new DateTimeOffset(2026, 10, 1, 0, 0, 0, TimeSpan.Zero),
                UpdatedAt = new DateTimeOffset(2026, 10, 1, 0, 0, 0, TimeSpan.Zero)
            },
            new Permission
            {
                Id = Guid.Parse("36831268-086b-4635-975a-a99cf6e8e4da"),
                Name = "View Auth Log",
                Module = "Auth Log",
                Code = "authlog.read",
                CreatedAt = new DateTimeOffset(2026, 10, 1, 0, 0, 0, TimeSpan.Zero),
                UpdatedAt = new DateTimeOffset(2026, 10, 1, 0, 0, 0, TimeSpan.Zero)
            }
        );
    }
}