using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BallanceRecordApi.Data.Migrations
{
    public partial class AddTimeCreatedModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Time",
                table: "Records");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Duration",
                table: "Records",
                type: "time(6)",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeCreated",
                table: "Records",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeModified",
                table: "Records",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "TimeCreated",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "TimeModified",
                table: "Records");

            migrationBuilder.AddColumn<double>(
                name: "Time",
                table: "Records",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
