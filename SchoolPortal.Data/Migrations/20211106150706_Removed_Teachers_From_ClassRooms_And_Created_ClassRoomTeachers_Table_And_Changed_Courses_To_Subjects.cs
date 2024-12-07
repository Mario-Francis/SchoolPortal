using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SchoolPortal.Data.Migrations
{
    public partial class Removed_Teachers_From_ClassRooms_And_Created_ClassRoomTeachers_Table_And_Changed_Courses_To_Subjects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassRooms_Users_TeacherId",
                table: "ClassRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_EndTermResults_Courses_CourseId",
                table: "EndTermResults");

            migrationBuilder.DropForeignKey(
                name: "FK_MidTermResults_Courses_CourseId",
                table: "MidTermResults");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_MidTermResults_CourseId",
                table: "MidTermResults");

            migrationBuilder.DropIndex(
                name: "IX_EndTermResults_CourseId",
                table: "EndTermResults");

            migrationBuilder.DropIndex(
                name: "IX_ClassRooms_TeacherId",
                table: "ClassRooms");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "MidTermResults");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "EndTermResults");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "ClassRooms");

            migrationBuilder.AddColumn<long>(
                name: "SubjectId",
                table: "MidTermResults",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "SubjectId",
                table: "EndTermResults",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "ClassRoomTeachers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    ClassRoomId = table.Column<long>(nullable: false),
                    TeacherId = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassRoomTeachers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassRoomTeachers_ClassRooms_ClassRoomId",
                        column: x => x.ClassRoomId,
                        principalTable: "ClassRooms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClassRoomTeachers_Users_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ClassId = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subjects_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MidTermResults_SubjectId",
                table: "MidTermResults",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_EndTermResults_SubjectId",
                table: "EndTermResults",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassRoomTeachers_ClassRoomId",
                table: "ClassRoomTeachers",
                column: "ClassRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassRoomTeachers_TeacherId",
                table: "ClassRoomTeachers",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_ClassId",
                table: "Subjects",
                column: "ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_EndTermResults_Subjects_SubjectId",
                table: "EndTermResults",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MidTermResults_Subjects_SubjectId",
                table: "MidTermResults",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EndTermResults_Subjects_SubjectId",
                table: "EndTermResults");

            migrationBuilder.DropForeignKey(
                name: "FK_MidTermResults_Subjects_SubjectId",
                table: "MidTermResults");

            migrationBuilder.DropTable(
                name: "ClassRoomTeachers");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropIndex(
                name: "IX_MidTermResults_SubjectId",
                table: "MidTermResults");

            migrationBuilder.DropIndex(
                name: "IX_EndTermResults_SubjectId",
                table: "EndTermResults");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "MidTermResults");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "EndTermResults");

            migrationBuilder.AddColumn<long>(
                name: "CourseId",
                table: "MidTermResults",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "CourseId",
                table: "EndTermResults",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TeacherId",
                table: "ClassRooms",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassId = table.Column<long>(type: "bigint", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MidTermResults_CourseId",
                table: "MidTermResults",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_EndTermResults_CourseId",
                table: "EndTermResults",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassRooms_TeacherId",
                table: "ClassRooms",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_ClassId",
                table: "Courses",
                column: "ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassRooms_Users_TeacherId",
                table: "ClassRooms",
                column: "TeacherId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EndTermResults_Courses_CourseId",
                table: "EndTermResults",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MidTermResults_Courses_CourseId",
                table: "MidTermResults",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
