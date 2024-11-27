using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogMaster.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EspanishPurge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Blog_SlugEs",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "TagNameEs",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "KeywordNameEs",
                table: "Keywords");

            migrationBuilder.DropColumn(
                name: "CatergoryNameEs",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "ArticleEs",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "DescriptionEs",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "SlugEs",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "TitleEs",
                table: "Blogs");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("31d9ba1b-47f4-4a8a-98de-37ca4a1adec5"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "b474fef0-50ec-4abb-b065-52946084eea1", "AQAAAAIAAYagAAAAEHxsexhty4PZ3L2uYufTOn7ILvAfQeJEVIa19EMXgW/eHwzRM2jDNU2UtWaYRq4R3Q==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TagNameEs",
                table: "Tags",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KeywordNameEs",
                table: "Keywords",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CatergoryNameEs",
                table: "Categories",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArticleEs",
                table: "Blogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEs",
                table: "Blogs",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SlugEs",
                table: "Blogs",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TitleEs",
                table: "Blogs",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppSubscriptions",
                columns: table => new
                {
                    AppSubscriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CancelationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CustomerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NextBillingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubscriptionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSubscriptions", x => x.AppSubscriptionId);
                    table.ForeignKey(
                        name: "FK_AppSubscriptions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("31d9ba1b-47f4-4a8a-98de-37ca4a1adec5"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "0dc60405-3117-478f-b026-8871f4c29fb1", "AQAAAAIAAYagAAAAEI4Gr9XWEQ5NlqVC7+NXY0OMhhW+bJiMKpXjhgoVUq/Gek7+temXH65vxWINxjkbEg==" });

            migrationBuilder.CreateIndex(
                name: "IX_Blog_SlugEs",
                table: "Blogs",
                column: "SlugEs");

            migrationBuilder.CreateIndex(
                name: "IX_AppSubscriptions_UserId",
                table: "AppSubscriptions",
                column: "UserId");
        }
    }
}
