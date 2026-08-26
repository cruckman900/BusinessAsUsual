using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Platform.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMappingTemplateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MappingTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TableName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TemplateName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ConfigurationJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsShared = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MappingTemplates", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000001-0000-0000-0000-000000000001"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 26, 11, 14, 17, 816, DateTimeKind.Utc).AddTicks(9194));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000001-0000-0000-0000-000000000002"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 26, 11, 14, 17, 816, DateTimeKind.Utc).AddTicks(9295));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000001-0000-0000-0000-000000000003"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 26, 11, 14, 17, 816, DateTimeKind.Utc).AddTicks(9298));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000001-0000-0000-0000-000000000004"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 26, 11, 14, 17, 816, DateTimeKind.Utc).AddTicks(9300));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000002-0000-0000-0000-000000000001"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 26, 11, 14, 17, 816, DateTimeKind.Utc).AddTicks(9302));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000002-0000-0000-0000-000000000002"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 26, 11, 14, 17, 816, DateTimeKind.Utc).AddTicks(9308));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000002-0000-0000-0000-000000000003"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 26, 11, 14, 17, 816, DateTimeKind.Utc).AddTicks(9310));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000002-0000-0000-0000-000000000004"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 26, 11, 14, 17, 816, DateTimeKind.Utc).AddTicks(9312));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 26, 11, 14, 17, 816, DateTimeKind.Utc).AddTicks(2417));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 26, 11, 14, 17, 816, DateTimeKind.Utc).AddTicks(2513));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 26, 11, 14, 17, 816, DateTimeKind.Utc).AddTicks(2514));

            migrationBuilder.CreateIndex(
                name: "IX_MappingTemplates_CompanyId_TableName",
                table: "MappingTemplates",
                columns: new[] { "CompanyId", "TableName" });

            migrationBuilder.CreateIndex(
                name: "IX_MappingTemplates_TemplateName",
                table: "MappingTemplates",
                column: "TemplateName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MappingTemplates");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000001-0000-0000-0000-000000000001"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 26, 11, 2, 37, 815, DateTimeKind.Utc).AddTicks(114));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000001-0000-0000-0000-000000000002"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 26, 11, 2, 37, 815, DateTimeKind.Utc).AddTicks(290));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000001-0000-0000-0000-000000000003"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 26, 11, 2, 37, 815, DateTimeKind.Utc).AddTicks(293));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000001-0000-0000-0000-000000000004"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 26, 11, 2, 37, 815, DateTimeKind.Utc).AddTicks(296));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000002-0000-0000-0000-000000000001"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 26, 11, 2, 37, 815, DateTimeKind.Utc).AddTicks(298));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000002-0000-0000-0000-000000000002"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 26, 11, 2, 37, 815, DateTimeKind.Utc).AddTicks(306));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000002-0000-0000-0000-000000000003"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 26, 11, 2, 37, 815, DateTimeKind.Utc).AddTicks(309));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("00000002-0000-0000-0000-000000000004"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 26, 11, 2, 37, 815, DateTimeKind.Utc).AddTicks(311));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 26, 11, 2, 37, 814, DateTimeKind.Utc).AddTicks(1540));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 26, 11, 2, 37, 814, DateTimeKind.Utc).AddTicks(1646));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAt",
                value: new DateTime(2026, 8, 26, 11, 2, 37, 814, DateTimeKind.Utc).AddTicks(1648));
        }
    }
}
