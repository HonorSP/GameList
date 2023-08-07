using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameList.Migrations
{
    public partial class AddedCustomColors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BgColorBody",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BgColorDisplay",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BgColorFooter",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BgColorGameListLogo",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BgColorGameTile",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BgColorGamename",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BgColorSearch",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BgColorStatus",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BgColorStatusButton",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BgColorStatusButtonActive",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BgColorTable",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FontColorDisplayName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FontColorGamename",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FontColorSearch",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FontColorStatusButton",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FontColorTable",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BgColorBody",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BgColorDisplay",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BgColorFooter",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BgColorGameListLogo",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BgColorGameTile",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BgColorGamename",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BgColorSearch",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BgColorStatus",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BgColorStatusButton",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BgColorStatusButtonActive",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BgColorTable",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FontColorDisplayName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FontColorGamename",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FontColorSearch",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FontColorStatusButton",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FontColorTable",
                table: "AspNetUsers");
        }
    }
}
