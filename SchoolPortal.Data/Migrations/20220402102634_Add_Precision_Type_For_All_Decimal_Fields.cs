using Microsoft.EntityFrameworkCore.Migrations;

namespace SchoolPortal.Data.Migrations
{
    public partial class Add_Precision_Type_For_All_Decimal_Fields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "MidTermResults",
                type: "decimal(10, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TestScore",
                table: "MidTermResults",
                type: "decimal(10, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ExamScore",
                table: "MidTermResults",
                type: "decimal(10, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ClassWorkScore",
                table: "MidTermResults",
                type: "decimal(10, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "StartWeight",
                table: "HealthRecords",
                type: "decimal(10, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "StartHeight",
                table: "HealthRecords",
                type: "decimal(10, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "EndWeight",
                table: "HealthRecords",
                type: "decimal(10, 2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "EndHeight",
                table: "HealthRecords",
                type: "decimal(10, 2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "EndTermResults",
                type: "decimal(10, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TestScore",
                table: "EndTermResults",
                type: "decimal(10, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ExamScore",
                table: "EndTermResults",
                type: "decimal(10, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ClassWorkScore",
                table: "EndTermResults",
                type: "decimal(10, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "MidTermResults",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10, 2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TestScore",
                table: "MidTermResults",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10, 2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ExamScore",
                table: "MidTermResults",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10, 2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ClassWorkScore",
                table: "MidTermResults",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10, 2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "StartWeight",
                table: "HealthRecords",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10, 2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "StartHeight",
                table: "HealthRecords",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10, 2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "EndWeight",
                table: "HealthRecords",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10, 2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "EndHeight",
                table: "HealthRecords",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10, 2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "EndTermResults",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10, 2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TestScore",
                table: "EndTermResults",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10, 2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ExamScore",
                table: "EndTermResults",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10, 2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ClassWorkScore",
                table: "EndTermResults",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10, 2)");
        }
    }
}
