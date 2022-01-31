using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SchoolPortal.Data.Migrations
{
    public partial class Added_Seed_Data_For_Behavioural_Ratings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "BehaviouralRatings",
                columns: new[] { "Id", "Category", "CreatedBy", "CreatedDate", "Name" },
                values: new object[,]
                {
                    { 1L, "Affective", "SYSTEM", new DateTimeOffset(new DateTime(2022, 1, 23, 23, 43, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)), "Punctuality" },
                    { 16L, "Psychomotor", "SYSTEM", new DateTimeOffset(new DateTime(2022, 1, 23, 23, 43, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)), "Drawing/Painting" },
                    { 15L, "Psychomotor", "SYSTEM", new DateTimeOffset(new DateTime(2022, 1, 23, 23, 43, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)), "Musical Skills" },
                    { 14L, "Psychomotor", "SYSTEM", new DateTimeOffset(new DateTime(2022, 1, 23, 23, 43, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)), "Sports" },
                    { 13L, "Psychomotor", "SYSTEM", new DateTimeOffset(new DateTime(2022, 1, 23, 23, 43, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)), "Handwriting" },
                    { 12L, "Affective", "SYSTEM", new DateTimeOffset(new DateTime(2022, 1, 23, 23, 43, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)), "Orderliness" },
                    { 11L, "Affective", "SYSTEM", new DateTimeOffset(new DateTime(2022, 1, 23, 23, 43, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)), "Honesty" },
                    { 10L, "Affective", "SYSTEM", new DateTimeOffset(new DateTime(2022, 1, 23, 23, 43, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)), "Attentiveness" },
                    { 9L, "Affective", "SYSTEM", new DateTimeOffset(new DateTime(2022, 1, 23, 23, 43, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)), "Health" },
                    { 8L, "Affective", "SYSTEM", new DateTimeOffset(new DateTime(2022, 1, 23, 23, 43, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)), "Emotional Stability" },
                    { 7L, "Affective", "SYSTEM", new DateTimeOffset(new DateTime(2022, 1, 23, 23, 43, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)), "Helping Others" },
                    { 6L, "Affective", "SYSTEM", new DateTimeOffset(new DateTime(2022, 1, 23, 23, 43, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)), "Poise" },
                    { 5L, "Affective", "SYSTEM", new DateTimeOffset(new DateTime(2022, 1, 23, 23, 43, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)), "Obedience" },
                    { 4L, "Affective", "SYSTEM", new DateTimeOffset(new DateTime(2022, 1, 23, 23, 43, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)), "Leadership" },
                    { 3L, "Affective", "SYSTEM", new DateTimeOffset(new DateTime(2022, 1, 23, 23, 43, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)), "Politeness" },
                    { 2L, "Affective", "SYSTEM", new DateTimeOffset(new DateTime(2022, 1, 23, 23, 43, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)), "Neatness" },
                    { 17L, "Psychomotor", "SYSTEM", new DateTimeOffset(new DateTime(2022, 1, 23, 23, 43, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)), "Verbal Fluency" },
                    { 18L, "Psychomotor", "SYSTEM", new DateTimeOffset(new DateTime(2022, 1, 23, 23, 43, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)), "Handling Tools" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BehaviouralRatings",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "BehaviouralRatings",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "BehaviouralRatings",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "BehaviouralRatings",
                keyColumn: "Id",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "BehaviouralRatings",
                keyColumn: "Id",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "BehaviouralRatings",
                keyColumn: "Id",
                keyValue: 6L);

            migrationBuilder.DeleteData(
                table: "BehaviouralRatings",
                keyColumn: "Id",
                keyValue: 7L);

            migrationBuilder.DeleteData(
                table: "BehaviouralRatings",
                keyColumn: "Id",
                keyValue: 8L);

            migrationBuilder.DeleteData(
                table: "BehaviouralRatings",
                keyColumn: "Id",
                keyValue: 9L);

            migrationBuilder.DeleteData(
                table: "BehaviouralRatings",
                keyColumn: "Id",
                keyValue: 10L);

            migrationBuilder.DeleteData(
                table: "BehaviouralRatings",
                keyColumn: "Id",
                keyValue: 11L);

            migrationBuilder.DeleteData(
                table: "BehaviouralRatings",
                keyColumn: "Id",
                keyValue: 12L);

            migrationBuilder.DeleteData(
                table: "BehaviouralRatings",
                keyColumn: "Id",
                keyValue: 13L);

            migrationBuilder.DeleteData(
                table: "BehaviouralRatings",
                keyColumn: "Id",
                keyValue: 14L);

            migrationBuilder.DeleteData(
                table: "BehaviouralRatings",
                keyColumn: "Id",
                keyValue: 15L);

            migrationBuilder.DeleteData(
                table: "BehaviouralRatings",
                keyColumn: "Id",
                keyValue: 16L);

            migrationBuilder.DeleteData(
                table: "BehaviouralRatings",
                keyColumn: "Id",
                keyValue: 17L);

            migrationBuilder.DeleteData(
                table: "BehaviouralRatings",
                keyColumn: "Id",
                keyValue: 18L);
        }
    }
}
