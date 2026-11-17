using Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Data.Configurations;

public class RolePermissionConfig : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        // --- Table Name ---
        builder.ToTable("role_permission");

        // --- Composite Primary Key ---
        builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });

        // --- Property ---
        builder.Property(rp => rp.AssignedAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        // --- Relation Table ---
        builder.HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(rp => rp.AssignedBy)
            .OnDelete(DeleteBehavior.SetNull);

        // --- Index ---
        builder.HasIndex(rp => rp.PermissionId)
            .HasDatabaseName("ix_role_permission_permission_id");

        builder.HasIndex(rp => rp.AssignedBy)
            .HasDatabaseName("ix_role_permission_assigned_id");

        // --- Initial Data (Admin, Employee & Dashboard) ---
        builder.HasData(
            new RolePermission
            {
                RoleId = Guid.Parse("049072c7-a124-41b4-bb13-b47454934030"),
                PermissionId = Guid.Parse("9f028d8a-b926-4799-9975-9d2cdfc92a1d"),
                AssignedBy = Guid.Parse("cc42e7a1-b515-4e29-97b9-991c1df22365"),
                AssignedAt = new DateTimeOffset(2026, 10, 1, 0, 0, 0, TimeSpan.Zero)
            },
            new RolePermission
            {
                RoleId = Guid.Parse("049072c7-a124-41b4-bb13-b47454934030"),
                PermissionId = Guid.Parse("36831268-086b-4635-975a-a99cf6e8e4da"),
                AssignedBy = Guid.Parse("cc42e7a1-b515-4e29-97b9-991c1df22365"),
                AssignedAt = new DateTimeOffset(2026, 10, 1, 0, 0, 0, TimeSpan.Zero)
            },
            new RolePermission
            {
                RoleId = Guid.Parse("25743188-9b88-4103-a321-96e11f69d26f"),
                PermissionId = Guid.Parse("9f028d8a-b926-4799-9975-9d2cdfc92a1d"),
                AssignedBy = Guid.Parse("cc42e7a1-b515-4e29-97b9-991c1df22365"),
                AssignedAt = new DateTimeOffset(2026, 10, 1, 0, 0, 0, TimeSpan.Zero)
            }
        );
    }
}