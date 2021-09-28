using Microsoft.EntityFrameworkCore.Migrations;

namespace BallanceRecordApi.Data.Migrations
{
    public partial class RenameNameToRemark : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Records",
                newName: "Remark");

            // migrationBuilder.RenameColumn(
            //     name: "Remark",
            //     table: "AspNetRoles",
            //     newName: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Records",
                newName: "Remark");

            // migrationBuilder.RenameColumn(
            //     name: "Name",
            //     table: "AspNetRoles",
            //     newName: "Remark");
        }
    }
}
