using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogMaster.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UniqueTableNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("31d9ba1b-47f4-4a8a-98de-37ca4a1adec5"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "236ed1f4-e838-4e22-a979-9422d7f525d5", "AQAAAAIAAYagAAAAEG384XIuPaTziLMQ8tQWoBhDr/htPLw/4Nli8q3WhwLoPSg2By1tSpmBViuQaae3mQ==", "0e7ef2be-91f8-4b27-9caf-9c1bdc47e831" });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_TagNameEn",
                table: "Tags",
                column: "TagNameEn",
                unique: true,
                filter: "[TagNameEn] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Keywords_KeywordNameEn",
                table: "Keywords",
                column: "KeywordNameEn",
                unique: true,
                filter: "[KeywordNameEn] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CatergoryNameEn",
                table: "Categories",
                column: "CatergoryNameEn",
                unique: true,
                filter: "[CatergoryNameEn] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tags_TagNameEn",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Keywords_KeywordNameEn",
                table: "Keywords");

            migrationBuilder.DropIndex(
                name: "IX_Categories_CatergoryNameEn",
                table: "Categories");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("31d9ba1b-47f4-4a8a-98de-37ca4a1adec5"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b474fef0-50ec-4abb-b065-52946084eea1", "AQAAAAIAAYagAAAAEHxsexhty4PZ3L2uYufTOn7ILvAfQeJEVIa19EMXgW/eHwzRM2jDNU2UtWaYRq4R3Q==", null });
        }
    }
}
