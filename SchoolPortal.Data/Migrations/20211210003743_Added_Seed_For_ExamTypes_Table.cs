using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SchoolPortal.Data.Migrations
{
    public partial class Added_Seed_For_ExamTypes_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ExamTypes",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "Name" },
                values: new object[] { 1L, "SYSTEM", new DateTimeOffset(new DateTime(2021, 10, 29, 18, 38, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)), "Mid-Term" });

            migrationBuilder.InsertData(
                table: "ExamTypes",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "Name" },
                values: new object[] { 2L, "SYSTEM", new DateTimeOffset(new DateTime(2021, 10, 29, 18, 38, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)), "End-Term" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ExamTypes",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "ExamTypes",
                keyColumn: "Id",
                keyValue: 2L);
        }
    }
}
