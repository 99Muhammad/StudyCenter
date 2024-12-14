using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SCMS_back_end.Migrations
{
    /// <inheritdoc />
    public partial class updateonDeleteaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_AspNetUsers_UserId",
                table: "Announcements");

            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_Audiences_AudienceId",
                table: "Announcements");

            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_Courses_CourseId",
                table: "Certificates");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Schedules_ScheduleId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Subjects_SubjectId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Courses_CourseId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Students_StudentId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Teachers_Departments_DepartmentId",
                table: "Teachers");

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_AspNetUsers_UserId",
                table: "Announcements",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_Audiences_AudienceId",
                table: "Announcements",
                column: "AudienceId",
                principalTable: "Audiences",
                principalColumn: "AudienceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_Courses_CourseId",
                table: "Certificates",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Schedules_ScheduleId",
                table: "Courses",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "ScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Subjects_SubjectId",
                table: "Courses",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Courses_CourseId",
                table: "Payments",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Students_StudentId",
                table: "Payments",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Teachers_Departments_DepartmentId",
                table: "Teachers",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "DepartmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_AspNetUsers_UserId",
                table: "Announcements");

            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_Audiences_AudienceId",
                table: "Announcements");

            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_Courses_CourseId",
                table: "Certificates");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Schedules_ScheduleId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Subjects_SubjectId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Courses_CourseId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Students_StudentId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Teachers_Departments_DepartmentId",
                table: "Teachers");

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_AspNetUsers_UserId",
                table: "Announcements",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_Audiences_AudienceId",
                table: "Announcements",
                column: "AudienceId",
                principalTable: "Audiences",
                principalColumn: "AudienceId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_Courses_CourseId",
                table: "Certificates",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Schedules_ScheduleId",
                table: "Courses",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "ScheduleId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Subjects_SubjectId",
                table: "Courses",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "SubjectId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Courses_CourseId",
                table: "Payments",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Students_StudentId",
                table: "Payments",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Teachers_Departments_DepartmentId",
                table: "Teachers",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
