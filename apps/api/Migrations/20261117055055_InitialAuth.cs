using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class InitialAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "auth");

            migrationBuilder.CreateTable(
                name: "permission",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Module = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permission", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "role",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NormalizedName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsSystemRole = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    NormalizedEmail = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    FailedLoginAttempts = table.Column<int>(type: "integer", nullable: false),
                    LastLoginAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "auth_log",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ActorUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TargetType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: true),
                    Details = table.Column<string>(type: "jsonb", nullable: true),
                    DeviceInfo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    IpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_auth_log", x => x.Id);
                    table.ForeignKey(
                        name: "FK_auth_log_user_ActorUserId",
                        column: x => x.ActorUserId,
                        principalSchema: "auth",
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "email_verify_token",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    FailedAttempts = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UsedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RevokedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_email_verify_token", x => x.Id);
                    table.ForeignKey(
                        name: "FK_email_verify_token_user_UserId",
                        column: x => x.UserId,
                        principalSchema: "auth",
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "refresh_token",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RevokedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ReplacedByTokenId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByIp = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_token", x => x.Id);
                    table.ForeignKey(
                        name: "FK_refresh_token_refresh_token_ReplacedByTokenId",
                        column: x => x.ReplacedByTokenId,
                        principalSchema: "auth",
                        principalTable: "refresh_token",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_refresh_token_user_UserId",
                        column: x => x.UserId,
                        principalSchema: "auth",
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reset_password_token",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UsedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RevokedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reset_password_token", x => x.Id);
                    table.ForeignKey(
                        name: "FK_reset_password_token_user_UserId",
                        column: x => x.UserId,
                        principalSchema: "auth",
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "role_permission",
                schema: "auth",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    AssignedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_permission", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_role_permission_permission_PermissionId",
                        column: x => x.PermissionId,
                        principalSchema: "auth",
                        principalTable: "permission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_role_permission_role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "auth",
                        principalTable: "role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_role_permission_user_AssignedBy",
                        column: x => x.AssignedBy,
                        principalSchema: "auth",
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "user_role",
                schema: "auth",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    AssignedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_role", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_user_role_role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "auth",
                        principalTable: "role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_role_user_AssignedBy",
                        column: x => x.AssignedBy,
                        principalSchema: "auth",
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_user_role_user_UserId",
                        column: x => x.UserId,
                        principalSchema: "auth",
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "auth",
                table: "permission",
                columns: new[] { "Id", "Code", "CreatedAt", "Description", "Module", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("36831268-086b-4635-975a-a99cf6e8e4da"), "authlog.read", new DateTimeOffset(new DateTime(2026, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "Auth Log", "View Auth Log", new DateTimeOffset(new DateTime(2026, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("9f028d8a-b926-4799-9975-9d2cdfc92a1d"), "dashboard.read", new DateTimeOffset(new DateTime(2026, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "Dashboard", "View Dashboard", new DateTimeOffset(new DateTime(2026, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
                });

            migrationBuilder.InsertData(
                schema: "auth",
                table: "role",
                columns: new[] { "Id", "CreatedAt", "Description", "IsSystemRole", "Name", "NormalizedName", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("049072c7-a124-41b4-bb13-b47454934030"), new DateTimeOffset(new DateTime(2026, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Full access", true, "Admin", "ADMIN", new DateTimeOffset(new DateTime(2026, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("25743188-9b88-4103-a321-96e11f69d26f"), new DateTimeOffset(new DateTime(2026, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Only can access for employee purpose", true, "Employee", "EMPLOYEE", new DateTimeOffset(new DateTime(2026, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
                });

            migrationBuilder.InsertData(
                schema: "auth",
                table: "user",
                columns: new[] { "Id", "CreatedAt", "Email", "EmailConfirmed", "FailedLoginAttempts", "IsActive", "LastLoginAt", "LockoutEnd", "NormalizedEmail", "Password", "UpdatedAt" },
                values: new object[] { new Guid("cc42e7a1-b515-4e29-97b9-991c1df22365"), new DateTimeOffset(new DateTime(2026, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "admin@companyemail.com", true, 0, true, null, null, "ADMIN@COMPANYEMAIL.COM", "$2a$11$G22Jj5etqSwCYNxEGsbqIuAM4qBcRacbFPp0xTpIuEJDSQHBnKyoi", new DateTimeOffset(new DateTime(2026, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.InsertData(
                schema: "auth",
                table: "role_permission",
                columns: new[] { "PermissionId", "RoleId", "AssignedAt", "AssignedBy" },
                values: new object[,]
                {
                    { new Guid("36831268-086b-4635-975a-a99cf6e8e4da"), new Guid("049072c7-a124-41b4-bb13-b47454934030"), new DateTimeOffset(new DateTime(2026, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("cc42e7a1-b515-4e29-97b9-991c1df22365") },
                    { new Guid("9f028d8a-b926-4799-9975-9d2cdfc92a1d"), new Guid("049072c7-a124-41b4-bb13-b47454934030"), new DateTimeOffset(new DateTime(2026, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("cc42e7a1-b515-4e29-97b9-991c1df22365") },
                    { new Guid("9f028d8a-b926-4799-9975-9d2cdfc92a1d"), new Guid("25743188-9b88-4103-a321-96e11f69d26f"), new DateTimeOffset(new DateTime(2026, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("cc42e7a1-b515-4e29-97b9-991c1df22365") }
                });

            migrationBuilder.InsertData(
                schema: "auth",
                table: "user_role",
                columns: new[] { "RoleId", "UserId", "AssignedAt", "AssignedBy" },
                values: new object[] { new Guid("049072c7-a124-41b4-bb13-b47454934030"), new Guid("cc42e7a1-b515-4e29-97b9-991c1df22365"), new DateTimeOffset(new DateTime(2026, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("cc42e7a1-b515-4e29-97b9-991c1df22365") });

            migrationBuilder.CreateIndex(
                name: "ix_auth_log_actor_user_id",
                schema: "auth",
                table: "auth_log",
                column: "ActorUserId");

            migrationBuilder.CreateIndex(
                name: "ix_auth_log_created_at",
                schema: "auth",
                table: "auth_log",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "ix_auth_log_target_type_target_id",
                schema: "auth",
                table: "auth_log",
                columns: new[] { "TargetType", "TargetId" });

            migrationBuilder.CreateIndex(
                name: "ix_email_verify_token_user_id_expires_at",
                schema: "auth",
                table: "email_verify_token",
                columns: new[] { "UserId", "ExpiresAt" });

            migrationBuilder.CreateIndex(
                name: "ix_email_verify_token_user_id_token",
                schema: "auth",
                table: "email_verify_token",
                columns: new[] { "UserId", "Token" });

            migrationBuilder.CreateIndex(
                name: "ux_permission_code",
                schema: "auth",
                table: "permission",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_refresh_token_replace_by_token_id",
                schema: "auth",
                table: "refresh_token",
                column: "ReplacedByTokenId");

            migrationBuilder.CreateIndex(
                name: "ix_refresh_token_user_id_expires_at",
                schema: "auth",
                table: "refresh_token",
                columns: new[] { "UserId", "ExpiresAt" });

            migrationBuilder.CreateIndex(
                name: "ux_refresh_token_hash",
                schema: "auth",
                table: "refresh_token",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_reset_password_token_user_id",
                schema: "auth",
                table: "reset_password_token",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "ux_reset_password_token_token",
                schema: "auth",
                table: "reset_password_token",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_role_normalized_name",
                schema: "auth",
                table: "role",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_role_permission_assigned_id",
                schema: "auth",
                table: "role_permission",
                column: "AssignedBy");

            migrationBuilder.CreateIndex(
                name: "ix_role_permission_permission_id",
                schema: "auth",
                table: "role_permission",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "ux_user_normalized_email",
                schema: "auth",
                table: "user",
                column: "NormalizedEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_role_assigned_by",
                schema: "auth",
                table: "user_role",
                column: "AssignedBy");

            migrationBuilder.CreateIndex(
                name: "ix_user_role_role_id",
                schema: "auth",
                table: "user_role",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "auth_log",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "email_verify_token",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "refresh_token",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "reset_password_token",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "role_permission",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "user_role",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "permission",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "role",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "user",
                schema: "auth");
        }
    }
}
