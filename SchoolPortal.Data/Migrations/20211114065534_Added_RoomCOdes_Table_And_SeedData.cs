using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SchoolPortal.Data.Migrations
{
    public partial class Added_RoomCOdes_Table_And_SeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoomCodes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    Code = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomCodes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "RoomCodes",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedDate" },
                values: new object[] { 1L, "CHARITY", "SYSTEM", new DateTimeOffset(new DateTime(2021, 10, 29, 18, 38, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)) });

            migrationBuilder.InsertData(
                table: "RoomCodes",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedDate" },
                values: new object[] { 2L, "PEACE", "SYSTEM", new DateTimeOffset(new DateTime(2021, 10, 29, 18, 38, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)) });

            migrationBuilder.InsertData(
                table: "RoomCodes",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedDate" },
                values: new object[] { 3L, "LOVE", "SYSTEM", new DateTimeOffset(new DateTime(2021, 10, 29, 18, 38, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoomCodes");
        }
    }
}
