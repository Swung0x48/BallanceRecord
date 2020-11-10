using Microsoft.EntityFrameworkCore.Migrations;

namespace BallanceRecordApi.Data.Migrations
{
    public partial class ScoreAndTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "Records",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "Time",
                table: "Records",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Score",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "Time",
                table: "Records");
        }
    }
}
