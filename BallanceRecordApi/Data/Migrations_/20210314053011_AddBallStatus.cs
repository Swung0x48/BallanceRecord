using Microsoft.EntityFrameworkCore.Migrations;

namespace BallanceRecordApi.Data.Migrations
{
    public partial class AddBallStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "BallSpeed",
                table: "Records",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "IsBouncing",
                table: "Records",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BallSpeed",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "IsBouncing",
                table: "Records");
        }
    }
}
