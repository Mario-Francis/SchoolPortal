using Microsoft.EntityFrameworkCore.Migrations;

namespace SchoolPortal.Data.Migrations
{
    public partial class Made_TeacherID_Nullabke_In_ClassRooms_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassRooms_Users_TeacherId",
                table: "ClassRooms");

            migrationBuilder.AlterColumn<long>(
                name: "TeacherId",
                table: "ClassRooms",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassRooms_Users_TeacherId",
                table: "ClassRooms",
                column: "TeacherId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassRooms_Users_TeacherId",
                table: "ClassRooms");

            migrationBuilder.AlterColumn<long>(
                name: "TeacherId",
                table: "ClassRooms",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassRooms_Users_TeacherId",
                table: "ClassRooms",
                column: "TeacherId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
