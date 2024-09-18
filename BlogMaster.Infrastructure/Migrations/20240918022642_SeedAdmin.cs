using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BlogMaster.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("8227a36b-1542-4b8e-8020-e199d8c5025e"), null, "Editor", "EDITOR" },
                    { new Guid("9535c06c-27d7-42b0-8b10-e0202e6bf6b6"), null, "Visitor", "VISITOR" },
                    { new Guid("96dd4365-6144-4fad-bd98-9dfba1274cd6"), null, "Writter", "WRITTER" },
                    { new Guid("f2b1b83f-d0a8-4916-94ad-fde172bf1923"), null, "Administrator", "ADMINISTRATOR" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { new Guid("31d9ba1b-47f4-4a8a-98de-37ca4a1adec5"), 0, "f0dfa33f-1ef0-4df9-af6e-77c6d8bb6821", "example@example.com", false, null, null, false, null, "EXAMPLE@EXAMPLE.COM", "ADMIN", "AQAAAAIAAYagAAAAECXmWonoK0n0pVGll++BiiYR6lDNIgCLzUZdcrZYA1UBJyTYIaId3YXHmmXoWckDZw==", null, false, null, false, "Admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { new Guid("f2b1b83f-d0a8-4916-94ad-fde172bf1923"), new Guid("31d9ba1b-47f4-4a8a-98de-37ca4a1adec5") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("8227a36b-1542-4b8e-8020-e199d8c5025e"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("9535c06c-27d7-42b0-8b10-e0202e6bf6b6"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("96dd4365-6144-4fad-bd98-9dfba1274cd6"));

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("f2b1b83f-d0a8-4916-94ad-fde172bf1923"), new Guid("31d9ba1b-47f4-4a8a-98de-37ca4a1adec5") });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("f2b1b83f-d0a8-4916-94ad-fde172bf1923"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("31d9ba1b-47f4-4a8a-98de-37ca4a1adec5"));
        }
    }
}
