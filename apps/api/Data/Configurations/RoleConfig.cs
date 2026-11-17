using Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Data.Configurations;

public class RoleConfig : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        // --- Table Name ---
        builder.ToTable("role");

        // --- Primary Key ---
        builder.HasKey(r => r.Id);

        // --- Property ---
        builder.Property(r => r.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(r => r.NormalizedName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(r => r.Description)
            .HasMaxLength(500);

        builder.Property(r => r.IsSystemRole)
            .HasDefaultValue(false);

        builder.Property(r => r.CreatedAt)
            .HasColumnType("timestamp with time zone");

        builder.Property(r => r.UpdatedAt)
            .HasColumnType("timestamp with time zone");

        // --- Relation Table ---
        builder.HasMany(r => r.UserRoles)
            .WithOne(ur => ur.Role)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.RolePermissions)
            .WithOne(rp => rp.Role)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        // --- Unique Index ---
        builder.HasIndex(r => r.NormalizedName)
            .IsUnique()
            .HasDatabaseName("ux_role_normalized_name");

        // --- Initial Data ---
        builder.HasData(
            new Role
            {
                Id = Guid.Parse("049072c7-a124-41b4-bb13-b47454934030"),
                Name = "Admin",
                NormalizedName = "ADMIN",
                Description = "Full access",
                IsSystemRole = true,
                CreatedAt = new DateTimeOffset(2026, 10, 1, 0, 0, 0, TimeSpan.Zero),
                UpdatedAt = new DateTimeOffset(2026, 10, 1, 0, 0, 0, TimeSpan.Zero)
            },
            new Role
            {
                Id = Guid.Parse("25743188-9b88-4103-a321-96e11f69d26f"),
                Name = "Employee",
                NormalizedName = "EMPLOYEE",
                Description = "Only can access for employee purpose",
                IsSystemRole = true,
                CreatedAt = new DateTimeOffset(2026, 10, 1, 0, 0, 0, TimeSpan.Zero),
                UpdatedAt = new DateTimeOffset(2026, 10, 1, 0, 0, 0, TimeSpan.Zero)
            }
        );
    }
}