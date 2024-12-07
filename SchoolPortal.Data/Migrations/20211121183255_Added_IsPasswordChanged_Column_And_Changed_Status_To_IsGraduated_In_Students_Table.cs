using Microsoft.EntityFrameworkCore.Migrations;

namespace SchoolPortal.Data.Migrations
{
    public partial class Added_IsPasswordChanged_Column_And_Changed_Status_To_IsGraduated_In_Students_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Students");

            migrationBuilder.AddColumn<bool>(
                name: "IsGraduated",
                table: "Students",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPasswordChanged",
                table: "Students",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsGraduated",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "IsPasswordChanged",
                table: "Students");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Students",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
