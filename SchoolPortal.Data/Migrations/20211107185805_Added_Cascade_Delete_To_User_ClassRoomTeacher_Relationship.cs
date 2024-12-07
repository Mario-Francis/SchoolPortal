using Microsoft.EntityFrameworkCore.Migrations;

namespace SchoolPortal.Data.Migrations
{
    public partial class Added_Cascade_Delete_To_User_ClassRoomTeacher_Relationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassRoomTeachers_ClassRooms_ClassRoomId",
                table: "ClassRoomTeachers");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassRoomTeachers_Users_TeacherId",
                table: "ClassRoomTeachers");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassRoomTeachers_ClassRooms_ClassRoomId",
                table: "ClassRoomTeachers",
                column: "ClassRoomId",
                principalTable: "ClassRooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassRoomTeachers_Users_TeacherId",
                table: "ClassRoomTeachers",
                column: "TeacherId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassRoomTeachers_ClassRooms_ClassRoomId",
                table: "ClassRoomTeachers");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassRoomTeachers_Users_TeacherId",
                table: "ClassRoomTeachers");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassRoomTeachers_ClassRooms_ClassRoomId",
                table: "ClassRoomTeachers",
                column: "ClassRoomId",
                principalTable: "ClassRooms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassRoomTeachers_Users_TeacherId",
                table: "ClassRoomTeachers",
                column: "TeacherId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
