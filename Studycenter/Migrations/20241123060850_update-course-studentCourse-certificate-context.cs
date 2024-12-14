using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SCMS_back_end.Migrations
{
    /// <inheritdoc />
    public partial class updatecoursestudentCoursecertificatecontext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_Courses_CourseId",
                table: "Certificates");

            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_Students_StudentId",
                table: "Certificates");

            migrationBuilder.DropIndex(
                name: "IX_Certificates_CourseId",
                table: "Certificates");

            migrationBuilder.DropIndex(
                name: "IX_Certificates_StudentId",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Certificates");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "Certificates",
                newName: "StudentCourseId");

            migrationBuilder.AddColumn<int>(
                name: "AssignmentsScore",
                table: "StudentCourses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QuizzesScore",
                table: "StudentCourses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Mark",
                table: "Courses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "Audiences",
                columns: new[] { "AudienceId", "Name" },
                values: new object[,]
                {
                    { 1, "Course" },
                    { 2, "Teachers" },
                    { 3, "Students" }
                });

            migrationBuilder.InsertData(
                table: "WeekDays",
                columns: new[] { "WeekDayId", "Name" },
                values: new object[,]
                {
                    { 1, "Saturday" },
                    { 2, "Sunday" },
                    { 3, "Monday" },
                    { 4, "Tuesday" },
                    { 5, "Wednesday" },
                    { 6, "Thursday" },
                    { 7, "Friday" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_StudentCourseId",
                table: "Certificates",
                column: "StudentCourseId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_StudentCourses_StudentCourseId",
                table: "Certificates",
                column: "StudentCourseId",
                principalTable: "StudentCourses",
                principalColumn: "StudentCourseId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_StudentCourses_StudentCourseId",
                table: "Certificates");

            migrationBuilder.DropIndex(
                name: "IX_Certificates_StudentCourseId",
                table: "Certificates");

            migrationBuilder.DeleteData(
                table: "Audiences",
                keyColumn: "AudienceId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Audiences",
                keyColumn: "AudienceId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Audiences",
                keyColumn: "AudienceId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "WeekDays",
                keyColumn: "WeekDayId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "WeekDays",
                keyColumn: "WeekDayId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "WeekDays",
                keyColumn: "WeekDayId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "WeekDays",
                keyColumn: "WeekDayId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "WeekDays",
                keyColumn: "WeekDayId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "WeekDays",
                keyColumn: "WeekDayId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "WeekDays",
                keyColumn: "WeekDayId",
                keyValue: 7);

            migrationBuilder.DropColumn(
                name: "AssignmentsScore",
                table: "StudentCourses");

            migrationBuilder.DropColumn(
                name: "QuizzesScore",
                table: "StudentCourses");

            migrationBuilder.DropColumn(
                name: "Mark",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "StudentCourseId",
                table: "Certificates",
                newName: "StudentId");

            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "Certificates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_CourseId",
                table: "Certificates",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_StudentId",
                table: "Certificates",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_Courses_CourseId",
                table: "Certificates",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_Students_StudentId",
                table: "Certificates",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
