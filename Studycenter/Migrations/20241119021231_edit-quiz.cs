using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SCMS_back_end.Migrations
{
    /// <inheritdoc />
    public partial class editquiz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_QuizResults_QuizId",
                table: "QuizResults");

            migrationBuilder.CreateIndex(
                name: "IX_QuizResults_QuizId",
                table: "QuizResults",
                column: "QuizId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_QuizResults_QuizId",
                table: "QuizResults");

            migrationBuilder.CreateIndex(
                name: "IX_QuizResults_QuizId",
                table: "QuizResults",
                column: "QuizId",
                unique: true);
        }
    }
}
