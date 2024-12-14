using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SCMS_back_end.Migrations
{
    /// <inheritdoc />
    public partial class updatefields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Assignments");

            migrationBuilder.AddColumn<int>(
                name: "FullMark",
                table: "Assignments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Announcements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullMark",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Announcements");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Assignments",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
