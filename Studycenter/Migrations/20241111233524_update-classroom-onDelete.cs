using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SCMS_back_end.Migrations
{
    /// <inheritdoc />
    public partial class updateclassroomonDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Classrooms_ClassroomId",
                table: "Courses");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Classrooms_ClassroomId",
                table: "Courses",
                column: "ClassroomId",
                principalTable: "Classrooms",
                principalColumn: "ClassroomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Classrooms_ClassroomId",
                table: "Courses");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Classrooms_ClassroomId",
                table: "Courses",
                column: "ClassroomId",
                principalTable: "Classrooms",
                principalColumn: "ClassroomId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
