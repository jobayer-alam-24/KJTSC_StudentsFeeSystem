using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentsFeeSystem.Migrations
{
    /// <inheritdoc />
    public partial class feecoladded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Fee",
                table: "Students",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Fee",
                table: "Students");
        }
    }
}
