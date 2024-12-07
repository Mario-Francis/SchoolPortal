using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SchoolPortal.Data.Migrations
{
    public partial class Added_ExamTypes_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exams_Terms_TermId",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Exams");

            migrationBuilder.AddColumn<long>(
                name: "ExamTypeId",
                table: "Exams",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "ExamTypes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Exams_ExamTypeId",
                table: "Exams",
                column: "ExamTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_ExamTypes_ExamTypeId",
                table: "Exams",
                column: "ExamTypeId",
                principalTable: "ExamTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_Terms_TermId",
                table: "Exams",
                column: "TermId",
                principalTable: "Terms",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exams_ExamTypes_ExamTypeId",
                table: "Exams");

            migrationBuilder.DropForeignKey(
                name: "FK_Exams_Terms_TermId",
                table: "Exams");

            migrationBuilder.DropTable(
                name: "ExamTypes");

            migrationBuilder.DropIndex(
                name: "IX_Exams_ExamTypeId",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "ExamTypeId",
                table: "Exams");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Exams",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_Terms_TermId",
                table: "Exams",
                column: "TermId",
                principalTable: "Terms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
