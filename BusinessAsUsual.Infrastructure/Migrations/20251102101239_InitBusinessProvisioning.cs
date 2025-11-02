using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessAsUsual.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitBusinessProvisioning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    CompanyID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.CompanyID);
                });

            migrationBuilder.CreateTable(
                name: "CompanySettings",
                columns: table => new
                {
                    SettingID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SettingKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SettingValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanySettings", x => x.SettingID);
                    table.ForeignKey(
                        name: "FK_CompanySettings_Companies_CompanyID",
                        column: x => x.CompanyID,
                        principalTable: "Companies",
                        principalColumn: "CompanyID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanyUserRolePermissionGroups",
                columns: table => new
                {
                    PermissionGroupID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyUserRolePermissionGroups", x => x.PermissionGroupID);
                    table.ForeignKey(
                        name: "FK_CompanyUserRolePermissionGroups_Companies_CompanyID",
                        column: x => x.CompanyID,
                        principalTable: "Companies",
                        principalColumn: "CompanyID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanyUserRoles",
                columns: table => new
                {
                    RoleID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyUserRoles", x => x.RoleID);
                    table.ForeignKey(
                        name: "FK_CompanyUserRoles_Companies_CompanyID",
                        column: x => x.CompanyID,
                        principalTable: "Companies",
                        principalColumn: "CompanyID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanyUsers",
                columns: table => new
                {
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyUsers", x => x.UserID);
                    table.ForeignKey(
                        name: "FK_CompanyUsers_Companies_CompanyID",
                        column: x => x.CompanyID,
                        principalTable: "Companies",
                        principalColumn: "CompanyID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanyUserRolePermissionGroupAssignments",
                columns: table => new
                {
                    AssignmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionGroupID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyUserRolePermissionGroupAssignments", x => x.AssignmentID);
                    table.ForeignKey(
                        name: "FK_CompanyUserRolePermissionGroupAssignments_CompanyUserRolePermissionGroups_PermissionGroupID",
                        column: x => x.PermissionGroupID,
                        principalTable: "CompanyUserRolePermissionGroups",
                        principalColumn: "PermissionGroupID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyUserRolePermissionGroupAssignments_CompanyUserRoles_RoleID",
                        column: x => x.RoleID,
                        principalTable: "CompanyUserRoles",
                        principalColumn: "RoleID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanyUserRolePermissions",
                columns: table => new
                {
                    PermissionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyUserRolePermissions", x => x.PermissionID);
                    table.ForeignKey(
                        name: "FK_CompanyUserRolePermissions_CompanyUserRoles_RoleID",
                        column: x => x.RoleID,
                        principalTable: "CompanyUserRoles",
                        principalColumn: "RoleID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanyUserRoleSettings",
                columns: table => new
                {
                    SettingID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SettingKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SettingValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyUserRoleSettings", x => x.SettingID);
                    table.ForeignKey(
                        name: "FK_CompanyUserRoleSettings_CompanyUserRoles_RoleID",
                        column: x => x.RoleID,
                        principalTable: "CompanyUserRoles",
                        principalColumn: "RoleID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanyUserRoleAssignments",
                columns: table => new
                {
                    AssignmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyUserRoleAssignments", x => x.AssignmentID);
                    table.ForeignKey(
                        name: "FK_CompanyUserRoleAssignments_CompanyUserRoles_RoleID",
                        column: x => x.RoleID,
                        principalTable: "CompanyUserRoles",
                        principalColumn: "RoleID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyUserRoleAssignments_CompanyUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "CompanyUsers",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanyUserRolePermissionGroupAudits",
                columns: table => new
                {
                    AuditID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionGroupID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangeDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangedByUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyUserRolePermissionGroupAudits", x => x.AuditID);
                    table.ForeignKey(
                        name: "FK_CompanyUserRolePermissionGroupAudits_CompanyUserRolePermissionGroups_PermissionGroupID",
                        column: x => x.PermissionGroupID,
                        principalTable: "CompanyUserRolePermissionGroups",
                        principalColumn: "PermissionGroupID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyUserRolePermissionGroupAudits_CompanyUsers_ChangedByUserID",
                        column: x => x.ChangedByUserID,
                        principalTable: "CompanyUsers",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanyUserSettings",
                columns: table => new
                {
                    SettingID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SettingKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SettingValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyUserSettings", x => x.SettingID);
                    table.ForeignKey(
                        name: "FK_CompanyUserSettings_CompanyUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "CompanyUsers",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanyUserRolePermissionGroupMembers",
                columns: table => new
                {
                    GroupMemberID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionGroupID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyUserRolePermissionGroupMembers", x => x.GroupMemberID);
                    table.ForeignKey(
                        name: "FK_CompanyUserRolePermissionGroupMembers_CompanyUserRolePermissionGroups_PermissionGroupID",
                        column: x => x.PermissionGroupID,
                        principalTable: "CompanyUserRolePermissionGroups",
                        principalColumn: "PermissionGroupID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyUserRolePermissionGroupMembers_CompanyUserRolePermissions_PermissionID",
                        column: x => x.PermissionID,
                        principalTable: "CompanyUserRolePermissions",
                        principalColumn: "PermissionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanyUserRolePermissionOverrides",
                columns: table => new
                {
                    OverrideID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsGranted = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyUserRolePermissionOverrides", x => x.OverrideID);
                    table.ForeignKey(
                        name: "FK_CompanyUserRolePermissionOverrides_CompanyUserRolePermissions_PermissionID",
                        column: x => x.PermissionID,
                        principalTable: "CompanyUserRolePermissions",
                        principalColumn: "PermissionID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyUserRolePermissionOverrides_CompanyUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "CompanyUsers",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanyUserRolePermissionSettings",
                columns: table => new
                {
                    SettingID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SettingKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SettingValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyUserRolePermissionSettings", x => x.SettingID);
                    table.ForeignKey(
                        name: "FK_CompanyUserRolePermissionSettings_CompanyUserRolePermissions_PermissionID",
                        column: x => x.PermissionID,
                        principalTable: "CompanyUserRolePermissions",
                        principalColumn: "PermissionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanySettings_CompanyID",
                table: "CompanySettings",
                column: "CompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyUserRoleAssignments_RoleID",
                table: "CompanyUserRoleAssignments",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyUserRoleAssignments_UserID",
                table: "CompanyUserRoleAssignments",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyUserRolePermissionGroupAssignments_PermissionGroupID",
                table: "CompanyUserRolePermissionGroupAssignments",
                column: "PermissionGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyUserRolePermissionGroupAssignments_RoleID",
                table: "CompanyUserRolePermissionGroupAssignments",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyUserRolePermissionGroupAudits_ChangedByUserID",
                table: "CompanyUserRolePermissionGroupAudits",
                column: "ChangedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyUserRolePermissionGroupAudits_PermissionGroupID",
                table: "CompanyUserRolePermissionGroupAudits",
                column: "PermissionGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyUserRolePermissionGroupMembers_PermissionGroupID",
                table: "CompanyUserRolePermissionGroupMembers",
                column: "PermissionGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyUserRolePermissionGroupMembers_PermissionID",
                table: "CompanyUserRolePermissionGroupMembers",
                column: "PermissionID");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyUserRolePermissionGroups_CompanyID",
                table: "CompanyUserRolePermissionGroups",
                column: "CompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyUserRolePermissionOverrides_PermissionID",
                table: "CompanyUserRolePermissionOverrides",
                column: "PermissionID");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyUserRolePermissionOverrides_UserID",
                table: "CompanyUserRolePermissionOverrides",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyUserRolePermissions_RoleID",
                table: "CompanyUserRolePermissions",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyUserRolePermissionSettings_PermissionID",
                table: "CompanyUserRolePermissionSettings",
                column: "PermissionID");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyUserRoles_CompanyID",
                table: "CompanyUserRoles",
                column: "CompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyUserRoleSettings_RoleID",
                table: "CompanyUserRoleSettings",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyUsers_CompanyID",
                table: "CompanyUsers",
                column: "CompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyUserSettings_UserID",
                table: "CompanyUserSettings",
                column: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanySettings");

            migrationBuilder.DropTable(
                name: "CompanyUserRoleAssignments");

            migrationBuilder.DropTable(
                name: "CompanyUserRolePermissionGroupAssignments");

            migrationBuilder.DropTable(
                name: "CompanyUserRolePermissionGroupAudits");

            migrationBuilder.DropTable(
                name: "CompanyUserRolePermissionGroupMembers");

            migrationBuilder.DropTable(
                name: "CompanyUserRolePermissionOverrides");

            migrationBuilder.DropTable(
                name: "CompanyUserRolePermissionSettings");

            migrationBuilder.DropTable(
                name: "CompanyUserRoleSettings");

            migrationBuilder.DropTable(
                name: "CompanyUserSettings");

            migrationBuilder.DropTable(
                name: "CompanyUserRolePermissionGroups");

            migrationBuilder.DropTable(
                name: "CompanyUserRolePermissions");

            migrationBuilder.DropTable(
                name: "CompanyUsers");

            migrationBuilder.DropTable(
                name: "CompanyUserRoles");

            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
