using Microsoft.EntityFrameworkCore.Migrations;

namespace SchoolPortal.Data.Migrations
{
    public partial class Added_ClassId_And_ClassRoomId_To_Result_Tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Subjects",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "ClassId",
                table: "MidTermResults",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ClassRoomId",
                table: "MidTermResults",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ClassId",
                table: "EndTermResults",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ClassRoomId",
                table: "EndTermResults",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_MidTermResults_ClassId",
                table: "MidTermResults",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_MidTermResults_ClassRoomId",
                table: "MidTermResults",
                column: "ClassRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_EndTermResults_ClassId",
                table: "EndTermResults",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_EndTermResults_ClassRoomId",
                table: "EndTermResults",
                column: "ClassRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_EndTermResults_Classes_ClassId",
                table: "EndTermResults",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EndTermResults_ClassRooms_ClassRoomId",
                table: "EndTermResults",
                column: "ClassRoomId",
                principalTable: "ClassRooms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MidTermResults_Classes_ClassId",
                table: "MidTermResults",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MidTermResults_ClassRooms_ClassRoomId",
                table: "MidTermResults",
                column: "ClassRoomId",
                principalTable: "ClassRooms",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EndTermResults_Classes_ClassId",
                table: "EndTermResults");

            migrationBuilder.DropForeignKey(
                name: "FK_EndTermResults_ClassRooms_ClassRoomId",
                table: "EndTermResults");

            migrationBuilder.DropForeignKey(
                name: "FK_MidTermResults_Classes_ClassId",
                table: "MidTermResults");

            migrationBuilder.DropForeignKey(
                name: "FK_MidTermResults_ClassRooms_ClassRoomId",
                table: "MidTermResults");

            migrationBuilder.DropIndex(
                name: "IX_MidTermResults_ClassId",
                table: "MidTermResults");

            migrationBuilder.DropIndex(
                name: "IX_MidTermResults_ClassRoomId",
                table: "MidTermResults");

            migrationBuilder.DropIndex(
                name: "IX_EndTermResults_ClassId",
                table: "EndTermResults");

            migrationBuilder.DropIndex(
                name: "IX_EndTermResults_ClassRoomId",
                table: "EndTermResults");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "MidTermResults");

            migrationBuilder.DropColumn(
                name: "ClassRoomId",
                table: "MidTermResults");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "EndTermResults");

            migrationBuilder.DropColumn(
                name: "ClassRoomId",
                table: "EndTermResults");
        }
    }
}
