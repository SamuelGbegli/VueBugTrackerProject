using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VueBugTrackerProject.Server.Migrations
{
    /// <inheritdoc />
    public partial class _06042025 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Bugs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Bugs");
        }
    }
}
