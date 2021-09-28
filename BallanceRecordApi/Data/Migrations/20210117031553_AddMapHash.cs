using Microsoft.EntityFrameworkCore.Migrations;

namespace BallanceRecordApi.Data.Migrations
{
    public partial class AddMapHash : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MapHash",
                table: "Records",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MapHash",
                table: "Records");
        }
    }
}
