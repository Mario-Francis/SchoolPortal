using Microsoft.EntityFrameworkCore.Migrations;

namespace SchoolPortal.Data.Migrations
{
    public partial class Correct_Typo_In_AppDbContext_Config : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ClassRoomId",
                table: "Classes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Classes_ClassRoomId",
                table: "Classes",
                column: "ClassRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_ClassRooms_ClassRoomId",
                table: "Classes",
                column: "ClassRoomId",
                principalTable: "ClassRooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_ClassRooms_ClassRoomId",
                table: "Classes");

            migrationBuilder.DropIndex(
                name: "IX_Classes_ClassRoomId",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "ClassRoomId",
                table: "Classes");
        }
    }
}
