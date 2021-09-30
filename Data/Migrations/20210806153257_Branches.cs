using Microsoft.EntityFrameworkCore.Migrations;

namespace IIS_V4.Data.Migrations
{
    public partial class Branches : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserBranch",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<long>(
                name: "UserBranch",
                table: "AspNetUsers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserBranch",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "UserBranch",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
