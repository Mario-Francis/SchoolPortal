using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SchoolPortal.Data.Migrations
{
    public partial class Add_New_Table_For_Weekly_CourseWorks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourseWorks",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    ClassRoomId = table.Column<long>(nullable: false),
                    WeekNo = table.Column<int>(nullable: false),
                    From = table.Column<DateTimeOffset>(nullable: false),
                    To = table.Column<DateTimeOffset>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    FilePath = table.Column<string>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseWorks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseWorks_ClassRooms_ClassRoomId",
                        column: x => x.ClassRoomId,
                        principalTable: "ClassRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseWorks_ClassRoomId",
                table: "CourseWorks",
                column: "ClassRoomId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseWorks");
        }
    }
}
