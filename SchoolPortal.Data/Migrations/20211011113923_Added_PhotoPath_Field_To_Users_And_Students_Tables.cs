using Microsoft.EntityFrameworkCore.Migrations;

namespace SchoolPortal.Data.Migrations
{
    public partial class Added_PhotoPath_Field_To_Users_And_Students_Tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoPath",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoPath",
                table: "Students",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoPath",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PhotoPath",
                table: "Students");
        }
    }
}
