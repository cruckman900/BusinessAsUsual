using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModuleRegistry.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Modules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModuleId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Version = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApiBaseUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UiEntryPoint = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Permissions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Capabilities = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HealthUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenantMode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RegisteredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastHealthCheck = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HealthStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MobileUISpecUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MobileContractVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupportsMobile = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Modules_ModuleId",
                table: "Modules",
                column: "ModuleId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Modules");
        }
    }
}
