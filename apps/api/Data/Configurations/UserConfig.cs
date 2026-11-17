using Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Data.Configurations;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // --- Table Name ---
        builder.ToTable("user");

        // --- Primary Key ---
        builder.HasKey(u => u.Id);

        // --- Property ---
        builder.Property(u => u.Email)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(u => u.NormalizedEmail)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(u => u.Password)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(u => u.EmailConfirmed)
            .HasDefaultValue(false);

        builder.Property(u => u.IsActive)
            .HasDefaultValue(true);

        builder.Property(u => u.CreatedAt)
            .HasColumnType("timestamp with time zone");

        builder.Property(u => u.UpdatedAt)
            .HasColumnType("timestamp with time zone");

        builder.Property(u => u.LastLoginAt)
            .HasColumnType("timestamp with time zone");

        // --- Relation Table ---
        builder.HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.RefreshTokens)
            .WithOne(rt => rt.User)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // --- Unique Index ---
        builder.HasIndex(u => u.NormalizedEmail)
            .IsUnique()
            .HasDatabaseName("ux_user_normalized_email");

        // --- Initial Data ---
        builder.HasData(
            new User
            {
                Id = Guid.Parse("cc42e7a1-b515-4e29-97b9-991c1df22365"),
                Email = "admin@companyemail.com",
                NormalizedEmail = "ADMIN@COMPANYEMAIL.COM",
                Password = "$2a$11$G22Jj5etqSwCYNxEGsbqIuAM4qBcRacbFPp0xTpIuEJDSQHBnKyoi", // admin123
                EmailConfirmed = true,
                CreatedAt = new DateTimeOffset(2026, 10, 1, 0, 0, 0, TimeSpan.Zero),
                UpdatedAt = new DateTimeOffset(2026, 10, 1, 0, 0, 0, TimeSpan.Zero)
            }
        );
    }
}