using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogMaster.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StripeCustomerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StripeCustomerId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("31d9ba1b-47f4-4a8a-98de-37ca4a1adec5"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "StripeCustomerId" },
                values: new object[] { "0dc60405-3117-478f-b026-8871f4c29fb1", "AQAAAAIAAYagAAAAEI4Gr9XWEQ5NlqVC7+NXY0OMhhW+bJiMKpXjhgoVUq/Gek7+temXH65vxWINxjkbEg==", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StripeCustomerId",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("31d9ba1b-47f4-4a8a-98de-37ca4a1adec5"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "e9fec341-8213-40b2-bd77-ea2b740eb027", "AQAAAAIAAYagAAAAEGhoE6hfviBQbtsMMKBrLMj+CXgmMGllxpr3y0dA9uCxypBLmXPnvWQJHTcaHMoEzg==" });
        }
    }
}
