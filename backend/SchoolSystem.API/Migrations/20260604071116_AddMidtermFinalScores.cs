using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolSystem.API.Migrations
{
    /// <inheritdoc />
    public partial class AddMidtermFinalScores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "Grades",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FinalScore",
                table: "Grades",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MidtermScore",
                table: "Grades",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comments",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "FinalScore",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "MidtermScore",
                table: "Grades");
        }
    }
}
