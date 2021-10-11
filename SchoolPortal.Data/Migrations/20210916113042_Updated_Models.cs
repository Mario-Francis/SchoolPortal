using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SchoolPortal.Data.Migrations
{
    public partial class Updated_Models : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exams_ExamTypes_ExamTypeId",
                table: "Exams");

            migrationBuilder.DropTable(
                name: "ExamTypes");

            migrationBuilder.DropTable(
                name: "Results");

            migrationBuilder.DropIndex(
                name: "IX_Exams_ExamTypeId",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "ExamTypeId",
                table: "Exams");

            migrationBuilder.AddColumn<string>(
                name: "AdmissionNo",
                table: "Students",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BehaviouralRatings",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Category = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BehaviouralRatings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EndTermResults",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    ExamId = table.Column<long>(nullable: false),
                    CourseId = table.Column<long>(nullable: false),
                    StudentId = table.Column<long>(nullable: false),
                    ClassWorkScore = table.Column<decimal>(nullable: false),
                    TestScore = table.Column<decimal>(nullable: false),
                    ExamScore = table.Column<decimal>(nullable: false),
                    Total = table.Column<decimal>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EndTermResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EndTermResults_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EndTermResults_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EndTermResults_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Grades",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    From = table.Column<int>(nullable: false),
                    To = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HealthRecords",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    Session = table.Column<string>(nullable: true),
                    TermId = table.Column<long>(nullable: false),
                    StudentId = table.Column<long>(nullable: false),
                    StartHeight = table.Column<decimal>(nullable: false),
                    EndHeight = table.Column<decimal>(nullable: true),
                    StartWeight = table.Column<decimal>(nullable: false),
                    EndWeight = table.Column<decimal>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HealthRecords_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HealthRecords_Terms_TermId",
                        column: x => x.TermId,
                        principalTable: "Terms",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MidTermResults",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    ExamId = table.Column<long>(nullable: false),
                    CourseId = table.Column<long>(nullable: false),
                    StudentId = table.Column<long>(nullable: false),
                    ClassWorkScore = table.Column<decimal>(nullable: false),
                    TestScore = table.Column<decimal>(nullable: false),
                    ExamScore = table.Column<decimal>(nullable: false),
                    Total = table.Column<decimal>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MidTermResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MidTermResults_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MidTermResults_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MidTermResults_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PerformanceRemarks",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    ExamId = table.Column<long>(nullable: false),
                    StudentId = table.Column<long>(nullable: false),
                    TeacherRemark = table.Column<string>(nullable: true),
                    HeadTeacherRemark = table.Column<string>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerformanceRemarks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PerformanceRemarks_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PerformanceRemarks_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TermSections",
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
                    table.PrimaryKey("PK_TermSections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BehaviouralResults",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    Session = table.Column<string>(nullable: true),
                    TermId = table.Column<long>(nullable: false),
                    StudentId = table.Column<long>(nullable: false),
                    BehaviouralRatingId = table.Column<long>(nullable: false),
                    Score = table.Column<string>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BehaviouralResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BehaviouralResults_BehaviouralRatings_BehaviouralRatingId",
                        column: x => x.BehaviouralRatingId,
                        principalTable: "BehaviouralRatings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BehaviouralResults_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BehaviouralResults_Terms_TermId",
                        column: x => x.TermId,
                        principalTable: "Terms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SessionTermLogs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    Session = table.Column<string>(nullable: true),
                    TermId = table.Column<long>(nullable: false),
                    TermSectionId = table.Column<long>(nullable: false),
                    IsCurrent = table.Column<bool>(nullable: false),
                    StartDate = table.Column<DateTimeOffset>(nullable: false),
                    EndDate = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionTermLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SessionTermLogs_Terms_TermId",
                        column: x => x.TermId,
                        principalTable: "Terms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SessionTermLogs_TermSections_TermSectionId",
                        column: x => x.TermSectionId,
                        principalTable: "TermSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BehaviouralResults_BehaviouralRatingId",
                table: "BehaviouralResults",
                column: "BehaviouralRatingId");

            migrationBuilder.CreateIndex(
                name: "IX_BehaviouralResults_StudentId",
                table: "BehaviouralResults",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_BehaviouralResults_TermId",
                table: "BehaviouralResults",
                column: "TermId");

            migrationBuilder.CreateIndex(
                name: "IX_EndTermResults_CourseId",
                table: "EndTermResults",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_EndTermResults_ExamId",
                table: "EndTermResults",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_EndTermResults_StudentId",
                table: "EndTermResults",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_HealthRecords_StudentId",
                table: "HealthRecords",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_HealthRecords_TermId",
                table: "HealthRecords",
                column: "TermId");

            migrationBuilder.CreateIndex(
                name: "IX_MidTermResults_CourseId",
                table: "MidTermResults",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_MidTermResults_ExamId",
                table: "MidTermResults",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_MidTermResults_StudentId",
                table: "MidTermResults",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceRemarks_ExamId",
                table: "PerformanceRemarks",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceRemarks_StudentId",
                table: "PerformanceRemarks",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionTermLogs_TermId",
                table: "SessionTermLogs",
                column: "TermId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionTermLogs_TermSectionId",
                table: "SessionTermLogs",
                column: "TermSectionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BehaviouralResults");

            migrationBuilder.DropTable(
                name: "EndTermResults");

            migrationBuilder.DropTable(
                name: "Grades");

            migrationBuilder.DropTable(
                name: "HealthRecords");

            migrationBuilder.DropTable(
                name: "MidTermResults");

            migrationBuilder.DropTable(
                name: "PerformanceRemarks");

            migrationBuilder.DropTable(
                name: "SessionTermLogs");

            migrationBuilder.DropTable(
                name: "BehaviouralRatings");

            migrationBuilder.DropTable(
                name: "TermSections");

            migrationBuilder.DropColumn(
                name: "AdmissionNo",
                table: "Students");

            migrationBuilder.AddColumn<long>(
                name: "ExamTypeId",
                table: "Exams",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "ExamTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Results",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssessmentScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CourseId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ExamId = table.Column<long>(type: "bigint", nullable: false),
                    ExamScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Grade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudentId = table.Column<long>(type: "bigint", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Results", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Results_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Results_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Results_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Exams_ExamTypeId",
                table: "Exams",
                column: "ExamTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Results_CourseId",
                table: "Results",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Results_ExamId",
                table: "Results",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_Results_StudentId",
                table: "Results",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_ExamTypes_ExamTypeId",
                table: "Exams",
                column: "ExamTypeId",
                principalTable: "ExamTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
