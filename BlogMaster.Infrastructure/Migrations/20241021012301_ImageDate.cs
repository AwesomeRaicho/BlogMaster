using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogMaster.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ImageDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "BlogImages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("31d9ba1b-47f4-4a8a-98de-37ca4a1adec5"),
                columns: new[] { "ConcurrencyStamp", "EmailConfirmed", "PasswordHash" },
                values: new object[] { "e9fec341-8213-40b2-bd77-ea2b740eb027", true, "AQAAAAIAAYagAAAAEGhoE6hfviBQbtsMMKBrLMj+CXgmMGllxpr3y0dA9uCxypBLmXPnvWQJHTcaHMoEzg==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "BlogImages");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("31d9ba1b-47f4-4a8a-98de-37ca4a1adec5"),
                columns: new[] { "ConcurrencyStamp", "EmailConfirmed", "PasswordHash" },
                values: new object[] { "f0dfa33f-1ef0-4df9-af6e-77c6d8bb6821", false, "AQAAAAIAAYagAAAAECXmWonoK0n0pVGll++BiiYR6lDNIgCLzUZdcrZYA1UBJyTYIaId3YXHmmXoWckDZw==" });
        }
    }
}
