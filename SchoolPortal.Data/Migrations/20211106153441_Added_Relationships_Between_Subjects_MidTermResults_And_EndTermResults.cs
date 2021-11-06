using Microsoft.EntityFrameworkCore.Migrations;

namespace SchoolPortal.Data.Migrations
{
    public partial class Added_Relationships_Between_Subjects_MidTermResults_And_EndTermResults : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EndTermResults_Subjects_SubjectId",
                table: "EndTermResults");

            migrationBuilder.DropForeignKey(
                name: "FK_MidTermResults_Subjects_SubjectId",
                table: "MidTermResults");

            migrationBuilder.AddForeignKey(
                name: "FK_EndTermResults_Subjects_SubjectId",
                table: "EndTermResults",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MidTermResults_Subjects_SubjectId",
                table: "MidTermResults",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EndTermResults_Subjects_SubjectId",
                table: "EndTermResults");

            migrationBuilder.DropForeignKey(
                name: "FK_MidTermResults_Subjects_SubjectId",
                table: "MidTermResults");

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
    }
}
